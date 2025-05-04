using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public TileType type;

    [Header("DEBUG")]

    public Vector2 coverPosition;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void SetCoverPosition(Vector2 position) => coverPosition = position;

    /// <summary>
    /// Returns true if it's between two asked tiles.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    public bool IsBetweenTiles(Tile tile1, Tile tile2)
    {
        if (tile1.coordinates.x == tile2.coordinates.x)
            if ((tile1.coordinates.y + tile2.coordinates.y) / 2f == coverPosition.y)
                return true;

        if (tile1.coordinates.y == tile2.coordinates.y)
            if ((tile1.coordinates.x + tile2.coordinates.x) / 2f == coverPosition.x)
                return true;

        return false;
    }

    /// <summary>
    /// Returns true if it's between two coordinates, else return false.
    /// </summary>
    /// <param name="coordinates1"></param>
    /// <param name="coordinates2"></param>
    /// <returns></returns>
    public bool IsBetweenCoordinates(Coordinates coordinates1, Coordinates coordinates2)
    {
        if (coordinates1.x == coordinates2.x)
            if (Mathf.Approximately((coordinates1.y + coordinates2.y) / 2f, coverPosition.y))
                return true;

        if (coordinates1.y == coordinates2.y)
            if (Mathf.Approximately((coordinates1.x + coordinates2.x) / 2f, coverPosition.x))
                return true;
        
        return false;
    }

    /// <summary>
    /// Returns the coordinates on the other side of the cover.
    /// </summary>
    /// <param name="firstSideCoordinates"></param>
    /// <returns></returns>
    public Coordinates GetOtherSideCoordinates(Coordinates firstSideCoordinates) => new(
        (int)(coverPosition.x - (firstSideCoordinates.x - coverPosition.x)),
        (int)(coverPosition.y - (firstSideCoordinates.y - coverPosition.y)));

    /// <summary>
    /// Returns the tile type of the cover.
    /// </summary>
    /// <returns></returns>
    public TileType GetTileType() => type;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
}
