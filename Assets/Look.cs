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

    public List<Tile> LineOfSight(Tile targetTile)
    {
        return _pathfinding.LineOfSight(c.Tile(), targetTile).ToList();
    }

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

    public void SetAimAreaActive (bool value)
    {
        if(value) // True
        {
            aimArea = AimArea();
            foreach (Tile t in aimArea)
            {
                t.SetMaterial(Tile.TileMaterial.Range);
            }
        }
        else // False
        {
            foreach (Tile t in aimArea)
            {
                t.SetMaterial(Tile.TileMaterial.Basic);
            }
            aimArea.Clear();
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private List<Tile> AimArea()
    {
        List<Tile> temp = _pathfinding.AroundTiles(c.Tile(), range);

        if (Utils.IsVoidList(temp)) return new List<Tile>(); // EXIT : no tiles around

        return temp
            .Where(o => o.type == Tile.Type.Basic)
            .Where(o => HasSightOn(o))
            .ToList();
    }
}
