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

    public Tile Tile()
    {
        return _terrain.GetTile(move.x, move.y);
    }

    public int Team()
    {
        return infos.team;
    }

    public void EnableTilesFeedbacks()
    {
        move.EnableMoveArea();
        attack.EnableAttackTiles();
    }

    public void ClearTilesFeedbacks()
    {
        move.ClearAreaZone();
        attack.ClearAttackTiles();
    }

    public bool CanAttack()
    {
        return actionPoints.actionPoints >= attack.actionPointsCost;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
