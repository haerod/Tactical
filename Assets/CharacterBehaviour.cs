using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class CharacterBehaviour : MonoBehaviour
{
    public enum Behaviour { Follower }
    public Behaviour behaviour = Behaviour.Follower;

    public Character target;

    public void Follow()
    {
        Character c = _characters.currentCharacter;

        if (c.behaviour.target == null) return;

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

        if (Utils.IsVoidList(path))
        {
            Debug.LogError("no path");
            return;
        }

        c.move.MoveOnPath(path);
    }
}
