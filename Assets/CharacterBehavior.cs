using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class CharacterBehavior : MonoBehaviour
{
    public bool playable = true;

    // * None : pass turn
    // * Follower : follow target, if target
    // * AttackerOnce : find the closest target, attack it and ends turn
    // * Offensive : find a target and attack it until it doent't have any action points
    public enum Behavior { None, Follower, AttackerOnce, Offensive }
    public Behavior behavior = Behavior.None;

    public Character target;

    [SerializeField] private Character c = null;

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
                    () => _characters.NextTurn());
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
        if (c.behavior.target == null) // Exit : no target
        {
            Wait(2, 
                () => _characters.NextTurn());
            return;
        }

        if (target == c) // Common mistake ^^'
        {
            Debug.LogError("oops, target is character itself");
            return;
        }

        // Get pathfinding
        Tile endTile = _pathfinding.ClosestFreeTileWithShortestPath(c.Tile(), target.Tile());
        List<Tile> path = null;

        if (endTile) // If is an end tile (and different of current tile)
        {
            path = _pathfinding.Pathfind(c.Tile(), endTile);
        }

        if (Utils.IsVoidList(path))  // Exit : not path
        {
            Wait(2,
                () => _characters.NextTurn());
            return;
        }

        c.move.MoveOnPath(path, () => _characters.NextTurn()); // EXIT : move on path
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Choose the closest enemy target.
    /// </summary>
    private void AcquireTarget()
    {
        target = c.attack.ClosestEnemyOnSight();

        if(target == null || target.health.IsDead() || _characters.IsFinalTeam(c)) // EXIT : nobody in sight
        {
            _characters.NextTurn();
            return;
        }

        c.attack.AttackTarget(target, () => _characters.NextTurn());
    }

    /// <summary>
    /// Check it the character can attack and start it if it's possible.
    /// </summary>
    private void CheckOffensive()
    {
        if (_characters.IsFinalTeam(c)) // Victory
        {
            _characters.NextTurn();
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
                _characters.NextTurn();
            }
        }
        else // Find target
        {
            target = c.attack.ClosestEnemyOnSight();
            CheckOffensive();
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
