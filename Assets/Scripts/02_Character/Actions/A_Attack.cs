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
    
    private Action onAttackDone;

    public event EventHandler OnAttackStart;
    public event EventHandler OnAttackEnd;
    public event EventHandler<C__Character> OnAttackMiss;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the attackable tiles, depending on the rules.
    /// </summary>
    /// <returns></returns>
    public List<Tile> AttackableTiles() => c.look.EnemiesVisibleInFog()
            .Where(chara => IsInRange(chara.tile))
            .Select(chara => chara.tile)
            .ToList();

    /// <summary>
    /// Attacks the target and starts an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    public void Attack(C__Character currentTarget)
    {
        
        c.SetCanPlayValue(false);
        
        if (!c.look.CanSee(currentTarget)) 
            return; // Enemy not in sight

        C__Character target = currentTarget;

        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = c.weaponHolder.GetCurrentWeapon().GetDamages();

        OnAttackStart?.Invoke(this, EventArgs.Empty);
        _input.SetActivePlayerInput(false);

        c.anim.StartAttack();

        int percentOfTouch = GetChanceToTouch(
            c.look.GetTilesOfLineOfSightOn(currentTarget.coordinates).Count,
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
    public int GetChanceToTouch(int range, int protectionValue)
    {
        int precisionToReturn = precision - protectionValue;

        for (int i = 0; i < range; i++)
            precisionToReturn -= c.weaponHolder.GetCurrentWeapon().GetPrecisionMalusByDistance();
        
        return precisionToReturn < 0 ? 0 : precisionToReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
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
                target.health.AddDamages(damages, c.weaponHolder.GetCurrentWeapon().GetDamageTypes());
            }
            else
            {
                OnAttackMiss?.Invoke(this, target);
                target.anim.StartDodge();
            }
            
            Wait(0.5f, () =>
            {
                OnAttackEnd?.Invoke(this, EventArgs.Empty);
                Turns.EndTurn();
            });
        };
    }
    
    private bool IsInRange(Tile tile)
    {
        Weapon currentWeapon = c.weaponHolder.GetCurrentWeapon();
        List<Tile> los = c.look.GetTilesOfLineOfSightOn(tile.coordinates);

        if (currentWeapon.GetTouchInView())
            return c.look.CanSee(tile.character);
        if (currentWeapon.IsMeleeWeapon())
            return los.Count == 0;
        else
            return los.Count < currentWeapon.GetRange();
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================

    protected override void OnHoverEnemy(C__Character hoveredCharacter)
    {
        c.move.OrientTo(hoveredCharacter.transform.position);
        
        if(!IsInRange(hoveredCharacter.tile))
            return; // Enemy is not visible or not in range
        
        c.anim.StartAim();
    }

    protected override void OnExitCharacter(C__Character leftCharacter)
    {
        c.anim.StopAim();
    }
    
    protected override void OnClickAnyCharacter(C__Character clickedCharacter)
    {
        
        
        if(c.team.IsAllyOf(clickedCharacter)) 
            return; // Same team

        if(!IsInRange(clickedCharacter.tile))
            return; // Enemy is not visible or not in range
        
        // Attack
        Attack(clickedCharacter);
    }
}
