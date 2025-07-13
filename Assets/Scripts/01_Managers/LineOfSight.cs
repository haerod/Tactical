using System.Collections.Generic;
using UnityEngine;

public static class LineOfSight
{
    public enum TileInclusion { WithStart, WithEnd, WithStartAndEnd, WithoutStartAndEnd}
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns a line of sight from a tile to another one.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static List<Coordinates> GetLineOfSight(Tile startTile, Tile endTile, TileInclusion parameters = TileInclusion.WithoutStartAndEnd)
    {
        // Get Vector between start and end coordinates (Start tile - end tile)
        Vector2 v2 = new Vector2(endTile.coordinates.x - startTile.coordinates.x, endTile.coordinates.y - startTile.coordinates.y);

        // Get the length of distance
        float length = Mathf.Max(Mathf.Abs(v2.x), Mathf.Abs(v2.y));

        List<Coordinates> toReturn = new List<Coordinates>();

        if(parameters is TileInclusion.WithStart or TileInclusion.WithStartAndEnd)
        {
            toReturn.Add(startTile.coordinates);
        }

        for (int i = 1; i < length+1; i++)
        {
            // Theoretic coordinates of the segment
            Vector2 tile = new Vector2(startTile.coordinates.x, startTile.coordinates.y) + i / length * v2;
            Coordinates tileCoordinates =  new Coordinates(Mathf.RoundToInt(tile.x), Mathf.RoundToInt(tile.y));
            // if t is null, it's a hole
            toReturn.Add(tileCoordinates);
        }

        if (parameters is TileInclusion.WithStart or TileInclusion.WithoutStartAndEnd)
        {
            toReturn.Remove(endTile.coordinates);
        }

        return toReturn;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}