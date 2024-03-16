using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class TileAutoSnap : MonoBehaviour
{
    private Tile tile;
    private M_Board board;

    private Vector3 previousPosition;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        SetBaseParameters();
        SetParent();
        SetBaseCoordinates();
        AddToManager();
    }

    private void Update()
    {
        if (Application.isPlaying) return;
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        if(AreModifications())
        {
            MoveTileAt(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            AutofixTileSuperposition();
            UpdateModificationValues();
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying) return;
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        RemoveFromManagersList();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check for modifications in the position.
    /// </summary>
    /// <returns></returns>
    private bool AreModifications() => previousPosition != transform.position;

    /// <summary>
    /// Update the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousPosition = transform.position;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// When two tiles are at the same position, automatically reposition it at the closest free position.
    /// </summary>
    private void AutofixTileSuperposition()
    {
        Tile sameTile = board.grid
            .Where(o => o != tile && o.x == tile.x && o.y == tile.y)
            .FirstOrDefault();

        if (sameTile)
        {
            Vector2Int closestFreeCoordinates = board.GetClosestCoordinatesWithoutTile(tile.x, tile.y);
            MoveTileAt(closestFreeCoordinates.x, closestFreeCoordinates.y);
        }
    }

    /// <summary>
    /// Move tile position at the asked coordinates.
    /// Rename the element.
    /// Set the new elements dirty.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void MoveTileAt(int x, int y)
    {
        tile.x = x;
        tile.y = y;
        tile.transform.position = new Vector3(tile.x, 0, tile.y);

        tile.name = string.Format("{1},{2} - {0}", tile.type, tile.x, tile.y);
        tile.name = tile.name.Replace("(TileType)", "");

        EditorUtility.SetDirty(tile); // Save the tile modifications
        EditorUtility.SetDirty(tile.transform); // Save the tile modifications
        EditorUtility.SetDirty(tile.gameObject); // Save the tile modifications
    }

    /// <summary>
    /// Set the element parent.
    /// </summary>
    private void SetParent()
    {
        transform.parent = board.transform;
    }

    /// <summary>
    /// Set the basic parameters.
    /// </summary>
    private void SetBaseParameters()
    {
        tile = GetComponent<Tile>();
        board = FindAnyObjectByType<M_Board>();
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Add the element to its manager.
    /// </summary>
    private void AddToManager()
    {
        if (!board.grid.Contains(tile))
        {
            board.grid.Add(tile);
            EditorUtility.SetDirty(board);
        }
    }

    /// <summary>
    /// Set the base coordinates of the element.
    /// </summary>
    private void SetBaseCoordinates()
    {
        Tile tileAtCoordinates = board.GetTileAtCoordinates(tile.x, tile.y);

        if (tileAtCoordinates && tileAtCoordinates != tile)
        {
            Vector2Int freeCoordinates = board.GetClosestCoordinatesWithoutTile(tile.x, tile.y);
            tile.x = freeCoordinates.x;
            tile.y = freeCoordinates.y;

            previousPosition = new Vector3(freeCoordinates.x, 0, freeCoordinates.y);
            EditorUtility.SetDirty(this);
        }
    }

    /// <summary>
    /// Remove the object for its manager's list.
    /// </summary>
    private void RemoveFromManagersList()
    {
        if (board) // Called on exit of the prefab stage
        {
            board.grid.Remove(tile);
            EditorUtility.SetDirty(board);
        }
    }
}