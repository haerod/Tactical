using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C_Behavior : MonoBehaviour
{
    public bool playable = true;

    // * None : pass turn
    // * Follower : follow target, if target
    // * AttackerOnce : find the closest target, attack it and ends turn
    // * Offensive : find a target and attack it until it doent't have any action points
    public enum Behavior { None, Follower, Offensive}
    public Behavior behavior = Behavior.None;

    public C__Character target;

    [Header("REFERENCES")]

    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Start the character behavior.
    /// </summary>
    public void PlayBehavior()
    {
        switch (behavior)
        {
            case Behavior.None:
                c.SetCanPlayValue(false);
                Wait(1,
                    () => _turns.EndTurn());
                break;
            case Behavior.Follower:
                Wait(1, 
                    () => FollowTarget());
                break;
            case Behavior.Offensive:
                Wait(1, 
                    () => AcquireTarget());
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Follow the current target.
    /// </summary>
    public void FollowTarget()
    {
        if (c.behavior.target == null) // EXIT : no target
        {
            c.SetCanPlayValue(false);
            Wait(2, 
                () => _turns.EndTurn());
            return;
        }

        if (target == c) // Common mistake ^^'
        {
            Debug.LogError("oops, target is character itself");
            return;
        }

        // Get pathfinding
        Tile endTile = _pathfinding.ClosestFreeTileWithShortestPath(c.tile, target.tile, c.move.walakbleTiles);
        List<Tile> path = null;

        if (endTile) // If is an end tile (and different of current tile)
        {
            path = _pathfinding.Pathfind(c.tile, endTile, M_Pathfinding.TileInclusion.WithEnd, c.move.walakbleTiles);
        }

        if (Utils.IsVoidList(path))  // EXIT : not path
        {
            c.SetCanPlayValue(false);
            Wait(2,
                () => _turns.EndTurn());
            return;
        }

        c.move.MoveOnPath(path, () => _turns.EndTurn()); // EXIT : move on path
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Choose the closest enemy target.
    /// </summary>
    private void AcquireTarget()
    {
        target = c.look.ClosestEnemyOnSight();

        if(target == null || target.health.IsDead() || _characters.IsFinalTeam(c)) // EXIT : nobody in sight
        {
            c.SetCanPlayValue(false);
            _turns.EndTurn();
            return;
        }

        c.attack.Attack(target);
    }

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
