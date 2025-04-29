using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public static class Pathfinding
{
    public enum TileInclusion { WithStart, WithEnd, WithStartAndEnd, WithoutStartAndEnd} // Options of the line of sight

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the path.
    /// If no path, returns an empty list.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <param name="inclusion"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static List<Tile> GetPath(Tile startTile, Tile endTile, TileInclusion inclusion, MovementRules rules)
    {
        List<Tile> tilesToTest = new List<Tile>();
        List<Tile> alreadyTestedTiles = new List<Tile>();
        List<Tile> adjacentTiles = new List<Tile>();
        List<Tile> toReturn = new List<Tile>();

        if (!startTile) 
            return null; // Start tile is null
        if (!endTile) 
            return null; // End tile is null

        // Set first tile
        Tile currentTile = startTile;
        currentTile.cost = 0;
        tilesToTest.Add(currentTile);

        // Pathfinding loop
        while (tilesToTest.Count > 0)
        {
            // Choose current tile in open list
            currentTile = tilesToTest
                .OrderBy(o => o.f)
                .FirstOrDefault();
            alreadyTestedTiles.Add(currentTile);
            tilesToTest.Remove(currentTile);

            // Put adjacent tiles in around list
            adjacentTiles.Clear();
            adjacentTiles.AddRange(_board
                .GetTilesAround(currentTile, 1, rules.useDiagonals)
                .Except(alreadyTestedTiles)
                .Where(t => IsDirectionWalkable(t, currentTile, rules))
                .ToList());

            foreach (Tile tile in adjacentTiles)
            {
                // Calculate values
                tile.CalculateValues(endTile, currentTile);

                if (tile.cost > 0 && currentTile.cost + tile.GetCost(currentTile) < currentTile.cost)
                    tilesToTest.AddIfNew(tile);
            }

            // If it's end tile -> return path
            if (adjacentTiles.Contains(endTile))
            {
                alreadyTestedTiles.Add(endTile);
                endTile.parent = currentTile;

                // Add tiles from last to fist (without start and end)
                Tile t = endTile.parent;
                while (t != startTile)
                {
                    toReturn.Add(t);
                    t = t.parent;
                }

                toReturn.Reverse();

                if (inclusion is TileInclusion.WithStart or TileInclusion.WithStartAndEnd)
                    toReturn.Insert(0,startTile);
                if (inclusion is TileInclusion.WithEnd or TileInclusion.WithStartAndEnd)
                    toReturn.Add(endTile);

                tilesToTest.ForEach(tile => tile.ResetTileValues());
                alreadyTestedTiles.ForEach(tile => tile.ResetTileValues());
                return toReturn; // Found a path
            }

            adjacentTiles.ForEach(tile => tilesToTest.AddIfNew(tile));
        }

        tilesToTest.ForEach(tile => tile.ResetTileValues());
        alreadyTestedTiles.ForEach(tile => tile.ResetTileValues());
        return toReturn;
    }

    /// <summary>
    /// Return a line of sight from a tile to another one.
    /// Null tiles are holes.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static List<Coordinates> LineOfSight(Tile startTile, Tile endTile, TileInclusion parameters = TileInclusion.WithoutStartAndEnd)
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
            // if t is null, its a hole
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

    /// <summary>
    /// Return true if it's a path between the current and the tested tile.
    /// </summary>
    /// <param name="destinationTile"></param>
    /// <param name="currentTile"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    private static bool IsDirectionWalkable(Tile destinationTile, Tile currentTile, MovementRules rules)
    {
        if (rules.blockingCharacterTiles.Contains(destinationTile))
            return false; // Blocking character

        if (!rules.allowedTileTypes.Contains(destinationTile.type))
            return false; // Not walkable

        if (destinationTile.hasCovers)
            if (currentTile.IsCoverBetween(destinationTile, rules.allowedTileTypes))
                return false; // Cover between start and end and not walkable

        if (rules.useDiagonals)
            if (currentTile.IsDiagonalWith(destinationTile))
                if (IsDiagonalTileABlocker(destinationTile, currentTile, rules))
                    return false; // False : A tile in diagonal blocks the movement

        return true;
    }

    /// <summary>
    /// Return true if the path if the tiles in diagonal between the tested and current tile is a blocker.
    /// </summary>
    /// <param name="testedTile"></param>
    /// <param name="currentTile"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    private static bool IsDiagonalTileABlocker(Tile testedTile, Tile currentTile, MovementRules rules)
    {
        foreach (Tile t in _board.GetTilesOfASquareWithDiagonals(currentTile, testedTile))
        {
            if(!t)
                return true; // A diagonal is void

            if (!rules.allowedTileTypes.Contains(t.type))
                return true; // A diagonal contains a blocker

            if (testedTile.IsCoverBetween(t, rules.allowedTileTypes))
                return true; // A diagonal contains cover

            if (currentTile.IsCoverBetween(t, rules.allowedTileTypes))
                return true; // A diagonal contains cover
        }

        return false;
    }
}

public class MovementRules
{
    public readonly List<TileType> allowedTileTypes;
    public readonly List<Tile> blockingCharacterTiles;
    public readonly bool useDiagonals;

    public MovementRules(List<TileType> allowedTileTypes, List<Tile> blockingCharacterTiles, bool  useDiagonals)
    {
        this.allowedTileTypes = allowedTileTypes;
        this.blockingCharacterTiles = blockingCharacterTiles;
        this.useDiagonals = useDiagonals;
    }
}