using System.Collections.Generic;
using UnityEngine;

public static class LineOfSight
{
    public enum Including {Nothing, StartOnly, EndOnly, StartAndEnd}
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns a line of sight from a tile to another one.
    /// Uses a Bresenham's line algorithm.
    /// </summary>
    /// <param name="startCoordinate"></param>
    /// <param name="endCoordinate"></param>
    /// <param name="inclusion"></param>
    /// <returns></returns>
    public static List<Coordinates> GetLineOfSight(Coordinates startCoordinate, Coordinates endCoordinate, Including inclusion = Including.Nothing)
    {
        List<Coordinates> toReturn = new List<Coordinates>();
        
        // Get Vector between start and end coordinates (Start tile - end tile)
        Vector2 segmentDirection = new Vector2(endCoordinate.x - startCoordinate.x, endCoordinate.y - startCoordinate.y);
        // Get the length of distance
        float length = Mathf.Max(Mathf.Abs(segmentDirection.x), Mathf.Abs(segmentDirection.y));

        if(inclusion is Including.StartOnly or Including.StartAndEnd)
            toReturn.Add(startCoordinate);

        for (int i = 1; i < length+1; i++)
        {
            // Theoretic coordinates of the segment
            Vector2 theoreticalCoordinates = new Vector2(startCoordinate.x, startCoordinate.y) + i / length * segmentDirection;
            Coordinates realCoordinates =  new Coordinates(Mathf.RoundToInt(theoreticalCoordinates.x), Mathf.RoundToInt(theoreticalCoordinates.y));
            // if t is null, it's a hole
            toReturn.Add(realCoordinates);
        }
        
        if(inclusion is Including.EndOnly)
            return toReturn; // End only
        
        if (inclusion is Including.StartOnly or Including.Nothing)
            toReturn.Remove(endCoordinate);

        return toReturn;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}