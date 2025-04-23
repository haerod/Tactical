using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class TileAutoSnap : BaseAutoSnap
{
    [HideInInspector] public Tile tile; // Note : Let it serializable to be dirty.
    [HideInInspector] public M_Board board; // Note : Let it serializable to be dirty.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR

    private void Start()
    {
        if (!IsInEditor())
            return; // Not in editor

        if (IsOnValidPosition()) 
            return; // Not on a valid position
        
        isLocated = false;

        Vector2Int baseCoordinates = tile.coordinates;
        Vector2Int freeCoordinates = baseCoordinates;

        foreach (Vector2Int coordinates in board.GetEmptySquareCoordinates(tile.x, tile.y, 1))
        {
            if (GetOtherTileAtCoordinates(coordinates))
                continue; // Not empty

            if (tile.x == coordinates.x ^ tile.y == coordinates.y)
            {
                freeCoordinates = coordinates;
                break; // Not in diagonal, get it first.
            }
            
            freeCoordinates = coordinates;
        }

        if (freeCoordinates == baseCoordinates)
            return; // No free coordinates

        MoveObject(freeCoordinates);
        AddToManager();
        SetParametersDirty();
        isLocated = true;
    }

    private void OnDestroy()
    {
        if (!IsInEditor())
            return; // Not editor mode
        if (!board)
            return; // Exit prefab mode

        board.tileGrid.RemoveTile(tile);
        EditorUtility.SetDirty(board);
    }

#endif

    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override bool IsOnValidPosition() => !IsCollidingAnotherTile();
    protected override void MoveObject(Vector2Int coordinates)
    {
        tile.Setup(coordinates);
        tile.MoveAtGridPosition(coordinates.x, coordinates.y);
    }
    protected override void AddToManager() => board.tileGrid.AddTile(tile);
    protected override void RemoveFromManager() => board.tileGrid.RemoveTile(tile);
    protected override void SetParametersDirty()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(board);
    }
    protected override void SetParameters()
    {
        base.SetParameters();
        tile = GetComponent<Tile>();
        board = FindAnyObjectByType<M_Board>();
        transform.parent = board.transform;
        transform.hasChanged = true;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check if it collides a tile.
    /// </summary>
    /// <returns></returns>
    private bool IsCollidingAnotherTile() => GetOtherTileAtCoordinates(tile.coordinates);

    /// <summary>
    /// Return the first tile collided at given coordinates. 
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    private Tile GetOtherTileAtCoordinates(Vector2Int coordinates)
    {
        Collider[] colliders = Physics.OverlapBox(new Vector3(coordinates.x, 0, coordinates.y), Vector3.one * .4f);

        foreach (Collider collider in colliders)
        {
            Tile testedTile = collider.GetComponent<Tile>();

            if (!testedTile)
                continue; // No tile
            if (testedTile == tile)
                continue; // Same tile

            return tile;
        }

        return null;
    }
}