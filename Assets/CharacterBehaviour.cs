using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class CharacterBehaviour : MonoBehaviour
{
    public bool playable = true;

    public enum Behaviour { None, Follower }
    public Behaviour behaviour = Behaviour.Follower;

    public Character target;

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
                break;
            case Behaviour.Follower:
                Wait(1, 
                    () => FollowTarget());
                break;
            default:
                break;
        }
    }

    public void FollowTarget()
    {
        Character c = _characters.currentCharacter;

        if (c.behaviour.target == null) // Exit : no target
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

        List<Tile> path = new List<Tile>();
        path = _pathfinding.PathfindAround(
                c.GetTile(),
                c.behaviour.target.GetTile(),
                _rules.canPassAcross == M_GameRules.PassAcross.Nobody);

        if (Utils.IsVoidList(path))  // Exit : not path
        {
            Wait(2, 
                () => _characters.NextTurn());
            return;
        }

        c.move.MoveOnPath(path, () => _characters.NextTurn()); // Exit : move on path
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

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
