using UnityEngine;
using System.Collections;
using static M__Managers;

public class Character : MonoBehaviour
{
    public Move move;
    public Look look;
    public ActionPoints actionPoints;
    public Attack attack;
    public Health health;
    public Infos infos;
    public UI_SlicedHealthBar healthBar;
    public CharacterBehavior behavior;
    public AnimatorScripts anim; // With animator / skinned mesh renderer
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Return the tile of this character.
    /// </summary>
    /// <returns></returns>
    public Tile Tile()
    {
        return _terrain.GetTile(move.x, move.y);
    }

    /// <summary>
    /// Return the team of this character.
    /// </summary>
    /// <returns></returns>
    public int Team()
    {
        return infos.team;
    }

    /// <summary>
    /// Enable the feedbacks on the movable tiles and the attackable tiles.
    /// </summary>
    public void EnableTilesFeedbacks()
    {
        move.EnableMoveArea();
        attack.EnableAttackTiles();
    }

    /// <summary>
    /// Clear the feedbacks on the movable tiles and the attackable tiles and clear the linked lists.
    /// </summary>
    public void ClearTilesFeedbacks()
    {
        move.ClearAreaZone();
        attack.ClearAttackTiles();
    }

    /// <summary>
    /// Return true if the character has enough action points to attack.
    /// </summary>
    /// <returns></returns>
    public bool CanAttack()
    {
        return actionPoints.actionPoints >= attack.actionPointsCost;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
