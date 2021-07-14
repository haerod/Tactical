﻿using UnityEngine;
using System.Collections;
using static M__Managers;

public class Character : MonoBehaviour
{
    public Move move;
    public ActionPoints actionPoints;
    public Attack attack;
    public Health health;
    public CharacterBehaviour behaviour;
    public AnimatorScripts anim; // With animator
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public Tile Tile()
    {
        return _terrain.GetTile(move.x, move.y);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
