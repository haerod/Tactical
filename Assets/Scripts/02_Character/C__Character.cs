﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

public class C__Character : Entity
{
    [Header("NAME")]
    
    public string unitName = "Name";

    [Header("REFERENCES")]
    
    public C_Actions actions;
    public A_Move move;
    public C_Look look;
    public A_Attack attack;
    public C_Cover cover;
    public C_Health health;
    public C_Team team;
    public C_Behavior behavior;
    public C_UnitUI unitUI;
    public C_AnimatorScripts anim; // With animator / skinned mesh renderer
    [Space]
    public C_WeaponHolder weaponHolder;

    public Team unitTeam => Team();
    public int movementRange => move.movementRange;
    public Tile tile => Tile();
    
    private bool hasPlayed = false;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Moves the character at coordinates in world space.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void MoveAt(int x, int y)
    {
        coordinates.x = x;
        coordinates.y = y;
        transform.position = new Vector3(x, 0, y);
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Returns the team of this character.
    /// </summary>
    /// <returns></returns>
    public Team Team() => team.team;

    /// <summary>
    /// Enables the feedbacks on the movable tiles and the attackable tiles.
    /// </summary>
    public void EnableTilesFeedbacks()
    {
        _feedback.SetFogVisualsActive(false);
        _feedback.ShowVisibleElements(look.VisibleTiles());

        if (!behavior.playable) 
            return; // NPC
        if(!CanPlay()) 
            return; // Can't play

        _feedback.ShowAttackableTiles(attack.AttackableTiles());
        _feedback.ShowMovementArea(move.MovementArea());
    }

    /// <summary>
    /// Clears the feedbacks on the movable tiles and the attackable tiles and clears the linked lists.
    /// </summary>
    public void HideTilesFeedbacks()
    {
        _feedback.HideMovementArea();
        _feedback.HideAttackableTiles();
    }

    /// <summary>
    /// Returns if the character has already played.
    /// </summary>
    /// <returns></returns>
    public bool CanPlay() => !hasPlayed;

    /// <summary>
    /// Sets hasPlay to true or false.
    /// </summary>
    /// <param name="value"></param>
    public void SetCanPlayValue(bool value)
    {
        hasPlayed = !value;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns the tile of this character.
    /// </summary>
    /// <returns></returns>
    private Tile Tile() => _board
            ? _board.GetTileAtCoordinates(coordinates.x, coordinates.y)
            : FindAnyObjectByType<M_Board>().GetTileAtCoordinates(coordinates.x, coordinates.y);
}
