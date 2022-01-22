using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class Look : MonoBehaviour
{
    [SerializeField] private Character c = null;
    [SerializeField] private int range = 2;

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
        //print(string.Format("start : {0}, count : {1}", los[0].name, los.Count));
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
            if (t.Character()) return true;
        }

        return false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
