using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static M__Managers;

public class M_Board : MonoBehaviour
{
    [Header("REFERENCES")]

    [SerializeField] private Transform charactersParent;

    [Header("DEBUG INFOS")]
    public List<Tile> grid;
    public static M_Board instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_TileBoard in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Return tile at (x,y) coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile GetTileAtCoordinates(int x, int y)
    {
        return grid.Where(o => o.x == x && o.y == y).FirstOrDefault();
    }

    /// <summary>
    /// Return a tile with an offset.
    /// Return null for a hole.
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Tile GetTileWithOffset(int xOffset, int yOffset, Tile tile)
    {
        return GetTileAtCoordinates(tile.x + xOffset, tile.y + yOffset);
    }

    /// <summary>
    /// Return all the existing tiles around a tile (holes included).
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="forceNoDiagonals"></param>
    /// <returns></returns>
    public List<Tile> GetTilesAround(Tile tile, bool forceNoDiagonals = false)
    {
        List<Tile> toReturn = new List<Tile>();
        bool useDiagonals = _rules.useDiagonals;

        if (forceNoDiagonals) // Force no diagonals
            useDiagonals = false;

        toReturn.Add(GetTileWithOffset(0, -1, tile));

        if (useDiagonals)
            toReturn.Add(GetTileWithOffset(-1, -1, tile));

        toReturn.Add(GetTileWithOffset(-1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetTileWithOffset(-1, 1, tile));

        toReturn.Add(GetTileWithOffset(0, 1, tile));

        if (useDiagonals)
            toReturn.Add(GetTileWithOffset(1, 1, tile));

        toReturn.Add(GetTileWithOffset(1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetTileWithOffset(1, -1, tile));

        toReturn = toReturn.Where(o => o != null).ToList();

        if (Utils.IsVoidList(toReturn)) return null;

        return toReturn;
    }

    /// <summary>
    /// Return all coordinates of the edges of a square of center x, y with a radius.
    /// A radius of 0 returns the coordinates x,y.
    /// </summary>
    /// <param name="xCenter"></param>
    /// <param name="yCenter"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<Vector2Int> GetEmptySquareCoordinates(int xCenter, int yCenter, int radius)
    {
        List<Vector2Int> toReturn = new List<Vector2Int>();

        radius = Mathf.Abs(radius);
        int edgeLenght = 2 * radius + 1;

        if(radius == 0) // Distance = 0
        {
            toReturn.Add(new Vector2Int(xCenter, yCenter));
            return toReturn;
        }

        for (int i = 0; i < edgeLenght; i++) // Top line
        {
            toReturn.Add(new Vector2Int(xCenter - radius, yCenter - radius + i));
        }

        for (int i = 0; i < edgeLenght; i++) // Bot line
        {
            toReturn.Add(new Vector2Int(xCenter + radius, yCenter - radius + i));
        }

        for (int i = 0; i < edgeLenght - 2; i++) // Left line
        {
            toReturn.Add(new Vector2Int(xCenter - (radius - 1) + i, yCenter - radius));
        }

        for (int i = 0; i < edgeLenght - 2; i++) // Right line
        {
            toReturn.Add(new Vector2Int(xCenter - (radius - 1) + i, yCenter + radius));
        }

        return toReturn;
    }

    /// <summary>
    /// Return all the coordinates of square's points of centrer x,y with a radius.
    /// A radius of 0 returns the coordinates x,y.
    /// </summary>
    /// <param name="xCenter"></param>
    /// <param name="yCenter"></param>
    /// <param name="radius"></param>
    /// <returns></returns>

    public List<Vector2Int> GetFullSquareCoordinates(int xCenter, int yCenter, int radius)
    {
        List<Vector2Int> toReturn = new List<Vector2Int>();

        radius = Mathf.Abs(radius);

        for (int i = 0; i < radius; i++)
        {
            toReturn.AddRange(GetEmptySquareCoordinates(xCenter, yCenter, i));
        }

        return toReturn;
    }

    /// <summary>
    /// Returns the closest coordinates where there is no tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2Int GetClosestCoordinatesWithoutTile(int x, int y)
    {
        Vector2Int toReturn = new Vector2Int(x,y);

        int distance = 1;

        while (toReturn.x == x && toReturn.y == y)
        {
            List<Vector2Int> aroundCoordinates = GetEmptySquareCoordinates(x, y, distance);

            foreach (Vector2Int coordinate in aroundCoordinates)
            {
                if (GetTileAtCoordinates(coordinate.x, coordinate.y)) continue;

                toReturn.x = coordinate.x;
                toReturn.y = coordinate.y;
            }

            distance++;
        }

        return toReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}