using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class C__Character : MonoBehaviour
{
    public C_Move move;
    public C_Look look;
    public C_Attack attack;
    public C_Health health;
    public C_Infos infos;
    public C_Behavior behavior;
    public C_AnimatorScripts anim; // With animator / skinned mesh renderer
    public UI_SlicedHealthBar healthBar;

    public int team => Team();
    public int movementRange => move.movementRange;
    public int x => move.x;
    public int y => move.y;

    [HideInInspector] public Tile tile => Tile();
    private bool hasPlayed = false;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Return the team of this character.
    /// </summary>
    /// <returns></returns>
    public int Team()
    {
        return infos.team;
    }

    /// <summary>
    /// Return the playable teammates with action points.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> PlayableTeammatesWithActionPoints()
    {
        return _characters.GetTeam(this, true, true, true);
    }

    /// <summary>
    /// Enable the feedbacks on the movable tiles and the attackable tiles.
    /// </summary>
    public void EnableTilesFeedbacks(bool showTiles = true)
    {
        _feedback.SetViewLinesActive(false);
        look.EnableViewTiles();
        attack.EnableAttackTiles();

        if (!showTiles) return;

        move.EnableMoveArea();
    }

    /// <summary>
    /// Clear the feedbacks on the movable tiles and the attackable tiles and clear the linked lists.
    /// </summary>
    public void ClearTilesFeedbacks()
    {
        move.ClearAreaZone();
        attack.ClearAttackTiles();
    }

    public bool CanPlay()
    {
        return !hasPlayed;
    }

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
    private Tile Tile()
    {
        if(_board) // M__Managers isn't initialized in editor
            return _board.GetTileAtCoordinates(move.x, move.y);
        else
            return FindAnyObjectByType<M_Board>().GetTileAtCoordinates(move.x, move.y);
    }

}
