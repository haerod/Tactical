using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using static M__Managers;

public class M_Board : MonoBehaviour
{
    [Header("REFERENCES")]

    [SerializeField] private TileType groundType;
    [SerializeField] private TileType holeType;
    [SerializeField] private TileType bigObstacleType;
    [SerializeField] private Transform charactersParent;

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
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Bake the board tile and characters and set elements dirty.
    /// CALLED IN EDITOR
    /// </summary>
    public void BakeBoard()
    {
        grid.Clear();

        foreach (Transform item in transform)
        {
            Tile tile = item.GetComponent<Tile>();
            tile.x = Mathf.RoundToInt(item.position.x);
            tile.y = Mathf.RoundToInt(item.position.z);

            tile.name = string.Format("{1},{2} - {0}", tile.type, tile.x, tile.y);
            tile.name = tile.name.Replace("(TileType)", "");
            tile.transform.position = new Vector3(tile.x, 0, tile.y);

            if (!grid.Contains(tile))
                grid.Add(tile);

            // CHECK ERROR : Double tiles
            List<Tile> check = grid
                .Where(o => o != tile && o.x == tile.x && o.y == tile.y)
                .ToList();

            if (check.Count != 0)
            {
                foreach (Tile t in check)
                {
                    Debug.LogError(string.Format("The tile with coordinates {0}, {1} haves a double. One was deleted.", tile.x, tile.y), tile.gameObject);
                    grid.Remove(t);
                    DestroyImmediate(t.gameObject);
                }
            }

            EditorUtility.SetDirty(tile); // Save the tile modifications
            EditorUtility.SetDirty(tile.transform); // Save the tile modifications
            EditorUtility.SetDirty(tile.gameObject); // Save the tile modifications
        }

        foreach (Transform item in charactersParent)
        {
            C__Character chara = item.GetComponent<C__Character>();
            chara.move.x = Mathf.RoundToInt(item.position.x);
            chara.move.y = Mathf.RoundToInt(item.position.z);

            chara.transform.position = new Vector3(chara.move.x, 0, chara.move.y);
            chara.move.OrientToBasicPosition();

            EditorUtility.SetDirty(chara.move); // Save the character modifications
            EditorUtility.SetDirty(chara.anim); // Save the character modifications
            EditorUtility.SetDirty(chara.transform); // Save the character modifications
        }

        EditorUtility.SetDirty(this); // Save the grid modifications
    }

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

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}