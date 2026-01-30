using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static M__Managers;

[ExecuteInEditMode]
public class TileAutoSnap : BaseAutoSnap
{
    private Tile tile => _tile ??= GetComponent<Tile>();
    private Tile _tile;
    
#if UNITY_EDITOR
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override bool IsOnValidPosition() => !IsCollidingAnotherTile();
    
    protected override void MoveObject(Coordinates coordinates)
    {
        tile.Setup(coordinates);
        tile.MoveAtGridPosition(coordinates.x, coordinates.y);
    }
    
    protected override void SetParametersDirty() => EditorUtility.SetDirty(tile.gameObject);

    protected override void SetParameters()
    {
        base.SetParameters();
        transform.parent = _board.transform;
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
    private Tile GetOtherTileAtCoordinates(Coordinates coordinates)
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
#endif
}