using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class M_Board : MonoBehaviour
{
    public bool editMode = false;

    [Header("BOARD PARAMETERS")]
    [Range(1, 100)]
    public int length = 5; // x
    [Range(1, 100)]
    public int width = 5; // y

    [Header("SPECIAL TILES")]
    public List<Vector2Int> holeCoordinates;
    public List<Vector2Int> bigObstaclesCoordinates;

    [Header("REFERENCES")]

    [SerializeField] private GameObject tile = null;
    [SerializeField] private Transform boardParent = null;

    [HideInInspector] public List<Tile> grid;
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

        // Other instructions
        GenerateBoard();
    }

    private void Start()
    {
        // Edit mode
        if (editMode)
        {
            _input.enabled = false;
            _creator.enabled = true;
            _ui.SetTurnPlayerUIActive(false);
        }
        else
        {
            _creator.enabled = false;
            _ui.SetDebugCoordinatesTextActive(false);
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Create a tile at given coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void CreateTileAtCoordinates(int x, int y)
    {
        CreateTile(x, y);

        // Get new extermities of the board.
        int xMin = _board.grid.OrderBy(o => o.x).FirstOrDefault().x;
        int xMax = _board.grid.OrderByDescending(o => o.x).FirstOrDefault().x;
        int yMin = _board.grid.OrderBy(o => o.y).FirstOrDefault().y;
        int yMax = _board.grid.OrderByDescending(o => o.y).FirstOrDefault().y;

        // Create a list of coordinates of all the tiles of the board.
        List<Vector2Int> theoricalCoordinates = new List<Vector2Int>();

        for (int i = xMin; i < xMax + 1; i++)
        {
            for (int j = yMin; j < yMax + 1; j++)
            {
                theoricalCoordinates.Add(new Vector2Int(i, j));
            }
        }

        // Create holes only at the new coordinates (tiles before the new tile and previous tiles).
        foreach (Vector2Int coordinates in theoricalCoordinates)
        {
            if (GetTile(coordinates.x, coordinates.y)) continue; // tile already exist

            CreateTile(coordinates.x, coordinates.y, Tile.Type.Hole);
        }
    }

    /// <summary>
    /// Return tile at (x,y) coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Tile GetTile(int x, int y)
    {
        return grid.Where(o => o.x == x && o.y == y).FirstOrDefault();
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
        if (!InBoardRange(xOffset, yOffset, tile))
        {
            return null;
        }

        return GetTile(tile.x + xOffset, tile.y + yOffset);
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

        if (forceNoDiagonals) // Force no diagonals
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

        toReturn = toReturn.Where(o => o != null).ToList();

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
        grid = new List<Tile>();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2Int coordinates = new Vector2Int(i, j);

                if (holeCoordinates.Contains(coordinates)) // Hole
                {
                    CreateTile(i, j, Tile.Type.Hole);
                    //stat.HideValues(); // uncomment this line in pathfinding debug mode
                }
                else if (bigObstaclesCoordinates.Contains(coordinates)) // Big obstacle
                {
                    CreateTile(i, j, Tile.Type.BigObstacle);
                    //stat.HideValues(); // uncomment this line in pathfinding debug mode
                }
                else // Basic
                {
                    CreateTile(i, j);
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
        if (tile.x + xOffset < grid.OrderBy(o => o.x).FirstOrDefault().x) return false; // xMin
        if (tile.x + xOffset >= grid.OrderByDescending(o => o.x).FirstOrDefault().x) return false; // xMax
        if (tile.y + yOffset < grid.OrderBy(o => o.y).FirstOrDefault().y) return false; // yMin
        if (tile.y + yOffset >= grid.OrderByDescending(o => o.y).FirstOrDefault().y) return false; // yMax

        return true;
    }

    /// <summary>
    /// Create a tile at coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreateTile(int x, int y, Tile.Type type = Tile.Type.Basic)
    {
        // Creation
        Tile instaTile = Instantiate(tile, new Vector3(x, 0, y), Quaternion.identity, boardParent).GetComponent<Tile>();
        instaTile.x = x;
        instaTile.y = y;
        instaTile.name = "tile " + x + ", " + y;
        _board.grid.Add(instaTile);

        //Type
        if (type == Tile.Type.Basic) return; // EXIT : Tile is basic

        instaTile.ChangeType(type);
    }
}