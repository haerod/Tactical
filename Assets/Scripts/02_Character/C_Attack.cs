using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class C_Attack : MonoBehaviour
{
    public Weapon currentWeapon;

    [Header("REFERENCES")]
    
    [SerializeField] private C__Character c = null;

    private Action OnAttackDone;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Return the attackable tiles, depending the rules.
    /// </summary>
    /// <returns></returns>
    public List<Tile> AttackableTiles() => 
        c.look.CharactersInView()
            .Where(chara => chara.team != c.team)
            .Where(chara => c.look.HasSightOn(chara.tile))
            .Select(chara => chara.tile)
            .ToList();

    /// <summary>
    /// Attack the target and start an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    /// <param name="OnEnd"></param>
    public void Attack(C__Character currentTarget)
    {
        c.SetCanPlayValue(false);

        // EXIT : Enemy isn't in sight
        if (!c.look.HasSightOn(currentTarget.tile)) return;

        C__Character target = currentTarget;

        c.HideTilesFeedbacks();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = UnityEngine.Random.Range(currentWeapon.damagesRange.x, currentWeapon.damagesRange.y + 1);

        _input.ClearFeedbacksAndValues();
        _input.SetActiveClick(false);

        _ui.SetActivePlayerUI_Action(false);

        c.anim.StartShoot();        

        if (UnityEngine.Random.Range(0, 101) < GetPercentToTouch(c.look.LineOfSight(currentTarget.tile).Count)) // SUCCESS
        {
            SetOnAttackDone(true, damages, target);
        }
        else // MISS
        {
            SetOnAttackDone(false, 0, target);
        }
    }

    /// <summary>
    /// End the attack.
    /// Called by C_AnimatorScripts, after the shoot animation.
    /// </summary>
    public void EndAttack()
    {
        _camera.Shake();
        _ui.SetActivePlayerUI_Action(true);
        OnAttackDone();

        // Muzzle flare

        GameObject muzzleFlash = c.weaponHolder.GetCurrentWeaponGraphics().muzzleFlash;

        if(muzzleFlash)
        {
            muzzleFlash.SetActive(true);
            Wait(0.2f, () => muzzleFlash.SetActive(false));
        }
    }

    /// <summary>
    /// Returns the percent of chance to touch the target, including the reduction by distance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int GetPercentToTouch(int range)
    {
        int toReturn = 100;

        for (int i = 0; i < range; i++)
        {
            toReturn -= _rules.percentReductionByDistance;
        }

        toReturn = Mathf.Clamp(toReturn, 0, 100);

        return toReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Start a wait for "time" seconds and execute an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="OnEnd"></param>
    private void Wait(float time, Action OnEnd)
    {
        StartCoroutine(Wait_Co(time, OnEnd));
    }

    /// <summary>
    /// Wait for "time" seconds and execute an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="OnEnd"></param>
    /// <returns></returns>
    IEnumerator Wait_Co(float time, Action OnEnd)
    {
        yield return new WaitForSeconds(time);

        OnEnd();
    }

    /// <summary>
    /// Set OnAttackDone lambda.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="damages"></param>
    /// <param name="OnEnd"></param>
    private void SetOnAttackDone(bool success, int damages, C__Character target)
    {
        OnAttackDone = () =>
        {
            if (success)
            {
                target.health.AddDamages(damages, currentWeapon.damageType);
            }
            else
            {
                target.anim.StartDodge();
                _feedback.ActionEffectFeedback("MISS", target.transform);
            }
            Wait(0.5f, () => {_turns.EndTurn();});
        };
    }
}
