using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

public class Unit : Entity
{
    [Header("NAME")]
    
    public string unitName = "Name";

    [Header("REFERENCES")]
    
    public Unit_ActionsHolder actionsHolder;
    public Unit_Graphics graphics;
    public Unit_AnimatorScripts anim; // With animator / skinned mesh renderer
    public Unit_Behavior behavior;
    public A_Attack attack;
    public Unit_Cover cover;
    public Unit_Health health;
    public Unit_Look look;
    public A_Move move;
    public Unit_Team team;
    public Unit_Inventory inventory;
    public Unit_WorldUI ui;
    public Unit_WeaponHolder weaponHolder;
    
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
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
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
