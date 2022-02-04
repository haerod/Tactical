using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class C_Look : MonoBehaviour
{
    [SerializeField] private C__Character c = null;
    [SerializeField] private int range = 5;

    [HideInInspector] public List<Tile> aimArea;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Retrurn true if the character has the tile on sight.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool HasSightOn(Tile tile)
    {
        List<Tile> los = LineOfSight(tile);

        if (AreObstacles(los))
            return false; // Exit : obstacles
        if (los.Count + 1 > range)
            return false; // Exit : out of range

        return true; // Exit : has sight on target
    }

    /// <summary>
    /// Return the line of sight of the character.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public List<Tile> LineOfSight(Tile targetTile)
    {
        return _pathfinding.LineOfSight(c.Tile(), targetTile).ToList();
    }

    /// <summary>
    /// Return true if the character has obstacles on its line of sight.
    /// </summary>
    /// <param name="lineOfSight"></param>
    /// <returns></returns>
    public bool AreObstacles(List<Tile> lineOfSight)
    {
        if (Utils.IsVoidList(lineOfSight)) return false;

        foreach (Tile t in lineOfSight)
        {
            if (t.type == Tile.Type.BigObstacle) return true;
            if (t.Character())
            {
                if (!t.Character().health.IsDead()) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Return the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public C__Character ClosestEnemyOnSight()
    {
        return _characters.characters
            .Where(o => o != c) // remove emitter
            .Where(o => o.Team() != c.Team()) // remove allies
            .Where(o => HasSightOn(o.Tile())) // get all enemies on sight
            .OrderBy(o => LineOfSight(o.Tile()).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
