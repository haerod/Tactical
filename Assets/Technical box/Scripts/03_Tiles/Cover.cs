using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Cover : MonoBehaviour
{
    [SerializeField] private TileType tileType;
    [SerializeField] private Tile tile;
    [SerializeField] private Edge edge;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        tile = GetComponent<Tile>();
        edge = GetComponent<Edge>();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the tile type.
    /// </summary>
    /// <returns></returns>
    public TileType GetCoveringTileType() => tileType;
    
    /// <summary>
    /// Returns the associated tile, or null if it's on an edge.
    /// </summary>
    /// <returns></returns>
    public Tile GetTile() => tile;
    
    /// <summary>
    /// Returns the associated edge element, or null if it's on a tile.
    /// </summary>
    /// <returns></returns>
    public Edge GetEdgeElement() => edge;

    /// <summary>
    /// Returns the world coordinates of the cover as a Vector2.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetWorldCoordinatesAsVector2()
    {
        if (edge) // Cover on edges
            return new Vector2(GetEdgeElement().elementPosition.x, GetEdgeElement().elementPosition.y);
        
        return GetTile().coordinates.ToVector2(); // Cover on tile
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
}
