using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Pathfinding : MonoBehaviour
{
    public static M_Pathfinding instance;

    public enum TileInclusion { WithStart, WithEnd, WithStartAndEnd, WithoutStartAndEnd} // Options of the line of sight

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
            Debug.LogError("There is more than one M_Pathfinding in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

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
    /// <param name="allowedTiles"></param>
    /// <returns></returns>
    public List<Tile> Pathfind(Tile startTile, Tile endTile, TileInclusion inclusion, MovementRules rules)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();
        List<Tile> aroundList = new List<Tile>();
        List<Tile> toReturn = new List<Tile>();
        Tile currentTile;

        if (startTile == null) 
        {
            Debug.LogError("tile is null !"); 
            return null; // ERROR: Start tile is null
        }
        if (endTile == null) 
        {
            Debug.LogError("end tile is null !"); 
            return null; // ERROR : End tile is null
        }

        // Set first tile
        currentTile = startTile;
        currentTile.cost = 0;
        openList.Add(currentTile);

        // Pathfinding loop
        while (openList.Count > 0)
        {
            // Choose current tile in open list
            currentTile = openList
                .OrderBy(o => o.f)
                .FirstOrDefault();
            closedList.Add(currentTile);
            openList.Remove(currentTile);

            // Put adjacent tiles in around list
            aroundList.Clear();
            aroundList.AddRange(_board
                .GetTilesAround(currentTile, 1, _rules.useDiagonals)
                .Except(closedList)
                .Where(t => IsPathWalkable(t, currentTile, rules))
                .ToList());

            foreach (Tile tile in aroundList)
            {
                // Caluclate values
                tile.CalulateValues(endTile, currentTile);

                if (tile.cost > 0 && currentTile.cost + tile.GetCost(currentTile) < currentTile.cost)
                    openList.AddIfNew(tile);
            }

            // If it's end tile -> return path
            if (aroundList.Contains(endTile))
            {
                closedList.Add(endTile);
                endTile.parent = currentTile;

                // Add tiles from last to fist (without start and end)
                Tile t = endTile.parent;
                while (t != startTile)
                {
                    toReturn.Add(t);
                    t = t.parent;
                }

                toReturn.Reverse();

                if (inclusion == TileInclusion.WithStart || inclusion == TileInclusion.WithStartAndEnd)
                    toReturn.Insert(0,startTile);
                if (inclusion == TileInclusion.WithEnd || inclusion == TileInclusion.WithStartAndEnd)
                    toReturn.Add(endTile);

                openList.ForEach(tile => tile.ResetTileValues());
                closedList.ForEach(tile => tile.ResetTileValues());
                return toReturn; // Found a path
            }

            aroundList.ForEach(tile => openList.AddIfNew(tile));
        }

        openList.ForEach(tile => tile.ResetTileValues());
        closedList.ForEach(tile => tile.ResetTileValues());
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
    public List<Tile> LineOfSight(Tile startTile, Tile endTile, TileInclusion parameters = TileInclusion.WithoutStartAndEnd)
    {
        // Get Vector between start and end coordinates (Start tile - end tile)
        Vector2 v2 = new Vector2(endTile.x - startTile.x, endTile.y - startTile.y);

        // Get the length of distance
        float length = Mathf.Max(Mathf.Abs(v2.x), Mathf.Abs(v2.y));

        List<Tile> toReturn = new List<Tile>();

        if(parameters == TileInclusion.WithStart || parameters == TileInclusion.WithStartAndEnd)
        {
            toReturn.Add(startTile);
        }

        for (int i = 1; i < length+1; i++)
        {
            // Theoric coordinates of the segment
            Vector2 tile = new Vector2(startTile.x, startTile.y) + i / length * v2;
            Tile t = _board.GetTileAtCoordinates(Mathf.RoundToInt(tile.x), Mathf.RoundToInt(tile.y));
            // if t is null, its a hole
            toReturn.Add(t);
        }

        if (parameters == TileInclusion.WithStart || parameters == TileInclusion.WithoutStartAndEnd)
        {
            toReturn.Remove(endTile);
        }

        return toReturn; 
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Return true if it's a path between the current and the tested tile.
    /// </summary>
    /// <param name="testedTile"></param>
    /// <param name="currentTile"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    private bool IsPathWalkable(Tile testedTile, Tile currentTile, MovementRules rules)
    {
        if (rules.blockingCharacterTiles.Contains(testedTile))
            return false; // Blocking character

        if (!rules.allowedTileTypes.Contains(testedTile.type))
            return false; // Not walkable

        if (testedTile.hasCovers)
            if (currentTile.IsCoverBetween(testedTile, rules.allowedTileTypes))
                return false; // Cover between start and end and not walkable

        if (_rules.useDiagonals)
            if (currentTile.IsDiagonalWith(testedTile))
                if (IsDiagonalTileABlocker(testedTile, currentTile, rules))
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
    private bool IsDiagonalTileABlocker(Tile testedTile, Tile currentTile, MovementRules rules)
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
    public List<TileType> allowedTileTypes;
    public List<Tile> blockingCharacterTiles;

    public MovementRules(List<TileType> allowedTileTypes, List<Tile> blockingCharacterTiles)
    {
        this.allowedTileTypes = allowedTileTypes;
        this.blockingCharacterTiles = blockingCharacterTiles;
    }
}