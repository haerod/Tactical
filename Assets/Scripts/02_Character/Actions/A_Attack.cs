using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class A_Attack : A__Action
{
    [Header("PARAMETERS")]
    
    [SerializeField] private int precision = 100;
    [SerializeField] private Weapon currentWeapon;
    
    private Action onAttackDone;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the current weapon.
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon() => currentWeapon;
    
    /// <summary>
    /// Returns the attackable tiles, depending on the rules.
    /// </summary>
    /// <returns></returns>
    public List<Tile> AttackableTiles() => 
        c.look.CharactersVisibleInFog()
            .Where(chara => chara.unitTeam != c.unitTeam)
            .Where(chara => c.look.HasSightOn(chara.tile))
            .Select(chara => chara.tile)
            .ToList();

    /// <summary>
    /// Attacks the target and starts an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    public void Attack(C__Character currentTarget)
    {
        c.SetCanPlayValue(false);
        
        if (!c.look.HasSightOn(currentTarget.tile)) 
            return; // Enemy not in sight

        C__Character target = currentTarget;

        c.HideTilesFeedbacks();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = currentWeapon.GetDamages();

        _feedback.HideMovementFeedbacks();
        _ui.HidePercentText();
        _input.SetActiveClick(false);

        _ui.SetActivePlayerUI_Action(false);

        c.anim.StartAttack();

        int percentOfTouch = GetPercentToTouch(
            c.look.GetTilesOfLineOfSightOn(currentTarget.tile).Count,
            currentTarget.cover.GetCoverProtectionValueFrom(c.look));
        
        if (UnityEngine.Random.Range(0, 101) < percentOfTouch) // SUCCESS
            SetOnAttackDone(true, damages, target);
        else // MISS
            SetOnAttackDone(false, 0, target);
    }

    /// <summary>
    /// Ends the attack.
    /// Called by C_AnimatorScripts, after the shoot animation.
    /// </summary>
    public void EndAttack()
    {
        _camera.Shake();
        _ui.SetActivePlayerUI_Action(true);
        onAttackDone();

        // Muzzle flash

        GameObject muzzleFlash = c.weaponHolder.GetCurrentWeaponGraphics().GetMuzzleFlash();

        if (!muzzleFlash) 
            return; // No muzzle flash
        
        muzzleFlash.SetActive(true);
        Wait(0.2f, () => muzzleFlash.SetActive(false));
    }

    /// <summary>
    /// Returns the percent of chance to touch the target, including the reduction by distance.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="protectionValue"></param>
    /// <returns></returns>
    public int GetPercentToTouch(int range, int protectionValue)
    {
        int precisionToReturn = precision - protectionValue;

        for (int i = 0; i < range; i++)
            precisionToReturn -= _rules.percentReductionByDistance;
        
        return precisionToReturn < 0 ? 0 : precisionToReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts a wait for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits coroutine.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }

    /// <summary>
    /// Sets OnAttackDone lambda.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="damages"></param>
    /// <param name="target"></param>
    private void SetOnAttackDone(bool success, int damages, C__Character target)
    {
        onAttackDone = () =>
        {
            if (success)
            {
                target.health.AddDamages(damages, currentWeapon.GetDamageTypes());
            }
            else
            {
                target.anim.StartDodge();
                _feedback.ActionEffectFeedback("MISS", target.transform);
            }
            Wait(0.5f, () => {Turns.EndTurn();});
        };
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    protected override void Input_OnCharacterClick(object sender, C__Character clickedCharacter)
    {
        if(c.team.IsAllyOf(clickedCharacter)) 
            return; // Same team

        // Attack
        Attack(clickedCharacter);
    }

    protected override void Input_OnTileEnter(object sender, Tile enteredTile)
    {
        c.move.OrientTo(enteredTile.transform.position);
        c.anim.StopAim();
        
        if(!enteredTile.character)
            return; // No character on the tile
        if(c.team.IsAllyOf(enteredTile.character))
            return; // Character is an ally
        if(!c.look.HasSightOn(enteredTile))
            return; // Enemy is not visible
        
        c.anim.StartAim();
    }
}
