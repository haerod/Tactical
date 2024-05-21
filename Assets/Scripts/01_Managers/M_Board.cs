using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static M__Managers;

public class M_Board : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform charactersParent;

    public TileGrid tileGrid;
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
            Debug.LogError("There is more than one M_Board in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }

        if (Application.isPlaying)
            tileGrid.Setup();
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
    public Tile GetTileAtCoordinates(int x, int y) => tileGrid[x, y];

    /// <summary>
    /// Return true if a tile is in diagonal with another tile.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    public bool IsDiagonal(Tile tile1, Tile tile2)
    {
        if (tile1.x == tile2.x || tile1.y == tile2.y)
            return false;

        return true;
    }

    /// <summary>
    /// Return a tile with an offset.
    /// Return null for a hole.
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Tile GetTileWithOffset(int xOffset, int yOffset, Tile tile) => GetTileAtCoordinates(tile.x + xOffset, tile.y + yOffset);

    /// <summary>
    /// Return the tiles around the start tile, with a radius.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public List<Tile> GetTilesAround(Tile startTile, int distance, bool useDiagonals)
    {
        List<Tile> toReturn = new List<Tile>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                Tile tile = _board.GetTileAtCoordinates(x + startTile.x, y + startTile.y);

                if (!tile)
                    continue; // No tile
                if (tile == startTile)
                    continue; // Start tile
                if (useDiagonals)
                {
                    toReturn.Add(tile);
                    continue; // Not using diagonals
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);

                if (testDistance > distance)
                    continue; // Not in diagonal

                toReturn.Add(tile);
            }
        }

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

        if (radius == 0) // Distance = 0
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
    /// Returns the closest coordinates where there is no tile around, except the tile sending.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="except"></param>
    /// <returns></returns>
    public Vector2Int GetClosestFreePositionAround(Vector2Int coordinates, Vector3 selectionPosition, Tile senderTile)
    {
        Vector2Int toReturn = coordinates;

        int distance = 1;
        bool founded = false;
        float bestDistance = 0;

        while (!founded)
        {
            List<Vector2Int> aroundCoordinates = GetEmptySquareCoordinates(coordinates.x, coordinates.y, distance);

            foreach (Vector2Int testedCoordinate in aroundCoordinates)
            {
                Tile testedTile = GetTileAtCoordinates(testedCoordinate.x, testedCoordinate.y);

                if (testedTile && testedTile != senderTile) 
                    continue;

                founded = true;
                toReturn.x = testedCoordinate.x;
                toReturn.y = testedCoordinate.y;

                if (founded)
                {
                    if (Vector3.Distance(new Vector3(testedCoordinate.x, 0, testedCoordinate.y), selectionPosition) < bestDistance)
                    {
                        toReturn.x = testedCoordinate.x;
                        toReturn.y = testedCoordinate.y;
                    }
                }
                else
                {
                    founded = true;
                    toReturn.x = testedCoordinate.x;
                    toReturn.y = testedCoordinate.y;
                    bestDistance = Vector3.Distance(new Vector3(testedCoordinate.x, 0, testedCoordinate.y), selectionPosition);
                }
            }

            distance++;
        }

        return toReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
