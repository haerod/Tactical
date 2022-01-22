using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    stat.type = Tile.Type.Hole;
                    stat.DisableRenderer();
                    //stat.HideValues(); // uncomment this line in pathfinding debug mode
                }
                else if (bigObstaclesCoordinates.Contains(coordinates))
                {
                    stat.type = Tile.Type.BigObstacle;
                    stat.EnableBigObstacle();
                }
            }
        }
    }
}