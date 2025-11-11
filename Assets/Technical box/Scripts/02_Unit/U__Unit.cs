using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

public class U__Unit : Entity
{
    [Header("NAME")]
    
    public string unitName = "Name";

    [Header("REFERENCES")]
    
    public U_Actions actions;
    public U_AnimatorScripts anim; // With animator / skinned mesh renderer
    public U_Behavior behavior;
    public A_Attack attack;
    public U_Cover cover;
    public U_Health health;
    public U_Look look;
    public A_Move move;
    public U_Team team;
    public U_UnitUI unitUI;
    public U_WeaponHolder weaponHolder;
    
    public Team unitTeam => team.team;
    public Tile tile => Tile();
    
    private bool hasPlayed;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Moves the unit at coordinates in world space.
    /// </summary>
    /// <param name="newCoordinates"></param>
    public void MoveAt(Coordinates newCoordinates)
    {
        coordinates = newCoordinates;
        transform.position = coordinates.ToVector3();
        EditorUtility.SetDirty(this);
    }
    
    /// <summary>
    /// Returns if the unit has already played.
    /// </summary>
    /// <returns></returns>
    public bool CanPlay() => !hasPlayed;

    /// <summary>
    /// Sets hasPlay to true or false.
    /// </summary>
    /// <param name="value"></param>
    public void SetCanPlayValue(bool value) => hasPlayed = !value;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns the tile of this unit.
    /// </summary>
    /// <returns></returns>
    private Tile Tile() => _board
            ? _board.GetTileAtCoordinates(coordinates.x, coordinates.y)
            : FindAnyObjectByType<M_Board>().GetTileAtCoordinates(coordinates.x, coordinates.y);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
