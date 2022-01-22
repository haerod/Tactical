﻿using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class Attack : MonoBehaviour
{
    public int actionPointsCost = 3;
    public Vector2Int damagesRange = new Vector2Int(3, 5);
    public Character target;

    [Header("REFERENCES")]
    
    [SerializeField] private Character c = null;
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
    public void AttackTarget(Character currentTarget, Action OnEnd)
    {
        if (c.actionPoints.actionPoints < actionPointsCost)
        {
            OnEnd();
            return; // EXIT : No action points aviable
        }

        target = currentTarget;

        c.ClearTilesFeedbacks();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = UnityEngine.Random.Range(damagesRange.x, damagesRange.y + 1);

        _inputs.ClearFeedbacksAndValues();
        _inputs.SetClick(false);

        _ui.SetActionPlayerUIActive(false);

        c.anim.StartShoot();        
        c.actionPoints.RemoveActionPoints(actionPointsCost);

        OnAttackDone = () => 
        {
            target.health.AddDamages(damages);
            Wait(0.5f, () => 
            {
                _inputs.SetClick();
                c.EnableTilesFeedbacks();

                OnEnd();
            });
        };
    }

    /// <summary>
    /// End the attack.
    /// </summary>
    public void EndAttack()
    {
        _camera.Shake();
        _ui.SetActionPlayerUIActive(true);
        OnAttackDone();

        // Muzzle flare
        muzzleFlare.SetActive(true);
        Wait(0.1f, () => muzzleFlare.SetActive(false));
    }

    /// <summary>
    /// Return the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public Character ClosestEnemyOnSight()
    {
        Look look = c.look;

        return _characters.characters
            .Where(o => o != c) // remove emitter
            .Where(o => o.Team() != c.Team()) // remove allies
            .Where(o => look.HasSightOn(o.Tile())) // get all enemies on sight
            .OrderBy(o => look.LineOfSight(o.Tile()).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    }

    /// <summary>
    /// Find the attackable tiles and enable feeback on them.
    /// </summary>
    public void EnableAttackTiles()
    {
        if(c.actionPoints.actionPoints < c.attack.actionPointsCost)
        {
            return;
        }

        foreach (Character character in _characters.characters)
        {
            if (character == c) continue;
 
            if (character.Team() == c.Team()) continue;

            attackTiles.Add(character.Tile());
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
}
