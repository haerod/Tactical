using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class C_Attack : MonoBehaviour
{
    public Vector2Int damagesRange = new Vector2Int(3, 5);
    public C__Character target;

    [Header("REFERENCES")]
    
    [SerializeField] private C__Character c = null;
    [SerializeField] private GameObject muzzleFlare = null;

    private Action OnAttackDone;
    private List<Tile> attackTiles = new List<Tile>();

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Attack the target and start an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    /// <param name="OnEnd"></param>
    public void AttackTarget(C__Character currentTarget, Action OnEnd)
    {
        c.hasPlayed = true;

        // EXIT : Enemy isn't in sight
        if (!c.look.HasSightOn(currentTarget.tile)) return;

        target = currentTarget;

        c.ClearTilesFeedbacks();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = UnityEngine.Random.Range(damagesRange.x, damagesRange.y + 1);

        _input.ClearFeedbacksAndValues();
        _input.SetActiveClick(false);

        _ui.SetActivePlayerUI_Action(false);

        c.anim.StartShoot();        

        if (UnityEngine.Random.Range(0, 101) < GetPercentToTouch(c.look.LineOfSight(currentTarget.tile).Count)) // SUCCESS
        {
            SetOnAttackDone(true, damages, OnEnd);
        }
        else // MISS
        {
            SetOnAttackDone(false, 0, OnEnd);
        }
    }

    /// <summary>
    /// End the attack.
    /// </summary>
    public void EndAttack()
    {
        _camera.Shake();
        _ui.SetActivePlayerUI_Action(true);
        OnAttackDone();

        // Muzzle flare
        muzzleFlare.SetActive(true);
        Wait(0.2f, () => muzzleFlare.SetActive(false));
    }

    /// <summary>
    /// Find the attackable tiles and enable feeback on them.
    /// </summary>
    public void EnableAttackTiles()
    {
        foreach (C__Character character in _characters.characters)
        {
            if (character == c) continue;
 
            if (character.Team() == c.Team()) continue;

            attackTiles.Add(character.tile);
        }

        foreach (Tile t in attackTiles)
        {
            t.SetMaterial(Tile.TileMaterial.Attackable);
        }
    }

    /// <summary>
    /// Reset the tile skin and clear the attaclable tiles list.
    /// </summary>
    public void ClearAttackTiles()
    {
        foreach (Tile t in attackTiles)
        {
            t.ResetTileSkin();
        }

        attackTiles.Clear();
    }

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
    private void SetOnAttackDone(bool success, int damages, Action OnEnd)
    {
        OnAttackDone = () =>
        {
            if (success)
            {
                target.health.AddDamages(damages);
            }
            else
            {
                target.anim.StartDodge();
                _feedback.ActionEffectFeedback("MISS", target.transform);
            }
            Wait(0.5f, () =>
            {
                _input.SetActiveClick();
                c.EnableTilesFeedbacks();

                OnEnd();
            });
        };
    }
}
