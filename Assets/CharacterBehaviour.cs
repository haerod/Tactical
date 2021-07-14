﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class CharacterBehaviour : MonoBehaviour
{
    public bool playable = true;

    public enum Behaviour { None, Follower, Attacker }
    public Behaviour behaviour = Behaviour.Follower;

    public Character target;

    [SerializeField] private Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void PlayBehaviour()
    {
        switch (behaviour)
        {
            case Behaviour.None:
                Wait(1,
                    () => _characters.NextTurn());
                break;
            case Behaviour.Follower:
                Wait(1, 
                    () => FollowTarget());
                break;
            case Behaviour.Attacker:
                Wait(1, 
                    () => AcquireTarget());
                break;
            default:
                break;
        }
    }

    public void FollowTarget()
    {
        Character currentCharacter = _characters.currentCharacter;

        if (currentCharacter.behaviour.target == null) // Exit : no target
        {
            Wait(2, 
                () => _characters.NextTurn());
            return;
        }

        if (target == currentCharacter) // Common mistake ^^'
        {
            Debug.LogError("oops, target is character itself");
            return;
        }

        List<Tile> path = new List<Tile>();
        path = _pathfinding.PathfindAround(
                currentCharacter.Tile(),
                currentCharacter.behaviour.target.Tile(),
                _rules.canPassAcross == M_GameRules.PassAcross.Nobody);

        if (Utils.IsVoidList(path))  // Exit : not path
        {
            Wait(2, 
                () => _characters.NextTurn());
            return;
        }

        currentCharacter.move.MoveOnPath(path, () => _characters.NextTurn()); // Exit : move on path
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void AcquireTarget()
    {
        target = c.attack.ClosestCharacterOnSight();

        if(target == null) // EXIT : nobody in sight
        {
            _characters.NextTurn();
            return;
        }

        c.attack.AttackTarget(target, () => _characters.NextTurn());
    }

    private void Wait(float time, Action OnEnd)
    {
        StartCoroutine(Wait_Co(time, OnEnd));
    }

    IEnumerator Wait_Co(float time, Action OnEnd)
    {
        yield return new WaitForSeconds(time);

        OnEnd();
    }
}
