using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using static M__Managers;

public class C__Character : Entity
{
    public C_Move move;
    public C_Look look;
    public C_Attack attack;
    public C_Cover cover;
    public C_Health health;
    public C_Infos infos;
    public C_Behavior behavior;
    public C_UnitUI unitUI;
    public C_AnimatorScripts anim; // With animator / skinned mesh renderer
    [Space]
    public C_WeaponHolder weaponHolder;

    public Team team
    {
        get { return Team(); }
    }

    public int movementRange
    {
        get { return move.movementRange; }
    }

    public int x
    {
        get { return coordinates.x; }
    }

    public int y
    {
        get { return coordinates.y; }
    }

    public Tile tile
    {
        get { return Tile(); }
    }

    private bool hasPlayed = false;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Move the character at coordinates in world space and set Move to values.
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
    /// Return the team of this character.
    /// </summary>
    /// <returns></returns>
    public Team Team()
    {
        return infos.team;
    }

    /// <summary>
    /// Enable the feedbacks on the movable tiles and the attackable tiles.
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
    /// Clear the feedbacks on the movable tiles and the attackable tiles and clear the linked lists.
    /// </summary>
    public void HideTilesFeedbacks()
    {
        _feedback.HideMovementArea();
        _feedback.HideAttackableTiles();
    }

    /// <summary>
    /// Return if the character has already played.
    /// </summary>
    /// <returns></returns>
    public bool CanPlay()
    {
        return !hasPlayed;
    }

    /// <summary>
    /// Set hasPlay to true or false;
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
    /// Return the tile of this character.
    /// </summary>
    /// <returns></returns>
    private Tile Tile() => _board
            ? _board.GetTileAtCoordinates(coordinates.x, coordinates.y)
            : FindAnyObjectByType<M_Board>().GetTileAtCoordinates(coordinates.x, coordinates.y);
}
