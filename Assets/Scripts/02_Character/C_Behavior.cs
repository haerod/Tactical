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
    public enum Behavior { None, Follower, AttackerOnce, Offensive }
    public Behavior behavior = Behavior.None;

    public C__Character target;

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
                Wait(1,
                    () => _turns.EndTurnOfTeamPCs());
                break;
            case Behavior.Follower:
                Wait(1, 
                    () => FollowTarget());
                break;
            case Behavior.AttackerOnce:
                Wait(1, 
                    () => AcquireTarget());
                break;
            case Behavior.Offensive:
                Wait(1, 
                    () => CheckOffensive());
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
            Wait(2, 
                () => _turns.EndTurnOfTeamPCs());
            return;
        }

        if (target == c) // Common mistake ^^'
        {
            Debug.LogError("oops, target is character itself");
            return;
        }

        // Get pathfinding
        Tile endTile = _pathfinding.ClosestFreeTileWithShortestPath(c.tile, target.tile);
        List<Tile> path = null;

        if (endTile) // If is an end tile (and different of current tile)
        {
            path = _pathfinding.Pathfind(c.tile, endTile, M_Pathfinding.TileInclusion.WithEnd);
        }

        if (Utils.IsVoidList(path))  // EXIT : not path
        {
            Wait(2,
                () => _turns.EndTurnOfTeamPCs());
            return;
        }

        c.move.MoveOnPath(path, () => _turns.EndTurnOfTeamPCs()); // EXIT : move on path
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
            _turns.EndTurnOfTeamPCs();
            return;
        }

        c.attack.AttackTarget(target, () => _turns.EndTurnOfTeamPCs());
    }

    /// <summary>
    /// Check if the character can attack and start the attack if it's possible.
    /// </summary>
    private void CheckOffensive()
    {
        if (_rules.actionsByTurn == M_Rules.ActionsByTurn.OneActionByTurn)
            AcquireTarget();

        if (_characters.IsFinalTeam(c)) // Victory
        {
            _turns.EndTurnOfTeamPCs();
            return;
        }

        if(target && !target.health.IsDead()) // Target
        {
            if(c.CanAttack()) // Attack
            {
                c.attack.AttackTarget(target, () => CheckOffensive());
            }
            else // Out AP
            {
                _turns.EndTurnOfTeamPCs();
            }
        }
        else // Find target
        {
            target = c.look.ClosestEnemyOnSight();

            if(target)
            {
                CheckOffensive();
            }
            else
            {
                _turns.EndTurnOfTeamPCs();
                return;
            }
        }
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
