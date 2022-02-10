using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class M_TileBoard : MonoBehaviour
{
    [Header("BOARD PARAMETERS")]
    [Range(1, 100)]
    public int length = 5;
    [Range(1, 100)]
    public int width = 5;

    [Header("SPECIAL TILES")]
    public List<Vector2Int> holeCoordinates;
    public List<Vector2Int> bigObstaclesCoordinates;

    [Header("REFERENCES")]

    [SerializeField] private GameObject tile = null;
    [SerializeField] private Transform boardParent = null;

    [HideInInspector] public Tile[,] grid;
    public static M_TileBoard instance;

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

        // Other instructions
        GenerateBoard();
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
    public Tile GetTile(int x, int y)
    {
        return grid[x, y];
    }

    /// <summary>
    /// Return a tile with an offset, if it exits in the board (holes included).
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public Tile GetOffsetTile(int xOffset, int yOffset, Tile tile)
    {
        if (!InBoardRange(xOffset, yOffset, tile)) return null;

        return grid[tile.x + xOffset, tile.y + yOffset];
    }

    /// <summary>
    /// Return all the existing tiles around a tile (holes included).
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="forceNoDiagonals"></param>
    /// <returns></returns>
    public List<Tile> GetAroundTiles(Tile tile, bool forceNoDiagonals = false)
    {
        List<Tile> toReturn = new List<Tile>();
        bool useDiagonals = _rules.useDiagonals;

        if (forceNoDiagonals) // Force no diagonals (aim zone for exemple)
            useDiagonals = false;

        toReturn.Add(GetOffsetTile(0, -1, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(-1, -1, tile));

        toReturn.Add(GetOffsetTile(-1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(-1, 1, tile));

        toReturn.Add(GetOffsetTile(0, 1, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(1, 1, tile));

        toReturn.Add(GetOffsetTile(1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(1, -1, tile));

        if (Utils.IsVoidList(toReturn)) return null;

        return toReturn;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Generate the board.
    /// </summary>
    private void GenerateBoard()
    {
        grid = new Tile[length, width];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 pozTile = new Vector3(i, 0, j);
                GameObject instaTile = Instantiate(tile, pozTile, Quaternion.identity, boardParent);
                instaTile.name = "tile " + i + ", " + j;

                Tile stat = instaTile.GetComponent<Tile>();
                stat.x = i;
                stat.y = j;
                grid[i, j] = stat;

                Vector2Int coordinates = new Vector2Int(i, j);
                if (holeCoordinates.Contains(coordinates))
                {
                    stat.EnableHole();
                    //stat.HideValues(); // uncomment this line in pathfinding debug mode
                }
                else if (bigObstaclesCoordinates.Contains(coordinates))
                {
                    stat.EnableBigObstacle();
                    //stat.HideValues(); // uncomment this line in pathfinding debug mode
                }
            }
        }
    }

    /// <summary>
    /// Return true if a tile is inside the board range.
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool InBoardRange(int xOffset, int yOffset, Tile tile)
    {
        if (tile.x + xOffset < 0) return false;
        if (tile.x + xOffset >= grid.GetLength(0)) return false;
        if (tile.y + yOffset < 0) return false;
        if (tile.y + yOffset >= grid.GetLength(1)) return false;

        return true;
    }
}