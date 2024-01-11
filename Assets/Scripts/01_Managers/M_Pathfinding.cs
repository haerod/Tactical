using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Pathfinding : MonoBehaviour
{
    private List<Tile> openList = new List<Tile>();
    private List<Tile> closedList = new List<Tile>();
    private List<Tile> aroundList = new List<Tile>();
    private Tile currentTile;
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
    /// Returns the path, exclding start tile and including the end tile.
    /// If no path, returns null.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <returns></returns>
    public List<Tile> Pathfind(Tile startTile, Tile endTile, TileInclusion inclusion, List<TileType> allowedTiles)
    {
        ClearPath();

        if (startTile == null) { Debug.LogError("tile is null !"); return null; } // EXIT ERROR : Tile is null
        if (endTile == null) { Debug.LogError("end tile is null !"); return null; } // EXIT ERROR : End tile is null

        // Set first tile
        currentTile = startTile;
        currentTile.cost = 0;
        openList.Add(currentTile);

        // Pathfinding loop
        while (openList.Count > 0)
        {            
            if (openList.Count == 0) return null; // Open list is void -> no direction in the path

            // Choose current tile in open list
            currentTile = openList.OrderBy(o => o.f).FirstOrDefault();
            closedList.Add(currentTile);
            openList.Remove(currentTile);

            // Put all tiles in around list
            aroundList.Clear();
            _board.
                GetTilesAround(currentTile).
                ForEach(tile => AddAllowedTile(tile, allowedTiles, endTile));

            // If it's end tile -> return path
            if (aroundList.Contains(endTile))
            {
                closedList.Add(endTile);
                List<Tile> toReturn = new List<Tile>();
                endTile.parent = currentTile;

                // Add tiles from end to start. After, reverse the list to have it from start to end.
                if(inclusion == TileInclusion.WithEnd || inclusion == TileInclusion.WithStartAndEnd)
                {
                    toReturn.Add(endTile);
                }
                while (endTile != startTile)
                {
                    Tile t = endTile.parent;
                    endTile = t;
                    toReturn.Add(endTile);
                }
                if (inclusion == TileInclusion.WithStart || inclusion == TileInclusion.WithStartAndEnd)
                {
                    toReturn.Add(startTile);
                }

                toReturn.Reverse();
                return toReturn;
            }

            aroundList.ForEach(tile => CalculateTileValues(tile, endTile));
        }

        return null;
    }

    /// <summary>
    /// Return the closest free tile from a target to its destination (with a maximum distance of this destination).
    /// Ex : a character wants the closest free tile around its target.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Tile ClosestFreeTileWithShortestPath(Tile from, Tile to, List<TileType> allowedTiles, int distance = 10)
    {
        if (from == null || to == null) return null; // EXIT : from or to doen't exist
        if (from == to) return null; // EXIT : from == to
        if (distance == 0) return null; // EXIT : no distance

        List<Tile> area = AreaMovementZone(to, distance, allowedTiles).ToList();

        // Get aviable tiles
        area = area
            .Where(o => !o.IsOccupiedByCharacter() || o == from) // remove occupied tiles or get the original tile
            .Where(o => allowedTiles.Contains(o.type)) // remove occupied tiles
            .OrderBy(o => o.cost) // order by cost
            .ToList();

        if(Utils.IsVoidList(area)) return null; // EXIT : no unoccupied or not hole tile around

        int lowest = area.FirstOrDefault().cost;

        // Get all the lowest cost tiles
        area = area
            .Where(o => o.cost == lowest)
            .ToList();

        if (area.Contains(from)) return null; // EXIT : "from" is the closest tile

        Tile toReturn = null;
        List<Tile> lowestPath = null;

        // Check the shortest path
        foreach (Tile tile in area)
        {
            List<Tile> testedPath = Pathfind(from, tile, TileInclusion.WithEnd, allowedTiles);

            if (testedPath == null) continue;

            if (lowestPath == null) // First aviable path
            {
                lowestPath = testedPath.ToList();
                toReturn = tile;
                continue;
            }

            if(testedPath.Count < lowestPath.Count) // Better path
            {
                lowestPath = testedPath.ToList();
                toReturn = tile;
                continue;
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Return the tiles around a tile (with a defined distance), considering holes and obstacles as blocking tiles.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<Tile> AreaMovementZone(Tile startTile, int distance, List<TileType> allowedTiles)
    {
        ClearPath();

        if (distance == 0)
        {
            return closedList;
        }

        // Set first tile
        currentTile = startTile;
        currentTile.cost = 0;
        openList.Add(currentTile);

        // Tile search loop
        while (openList.Count > 0)
        {
            if (openList.Count > 0) // Get first tile in open list
            {
                currentTile = openList[0];
                closedList.Add(currentTile);
                openList.Remove(currentTile);
            }
            else // Open list is void -> all direction founded
            {
                closedList.Remove(startTile);

                if (Utils.IsVoidList(closedList)) return null;

                return closedList;
            }

            // Put all tiles in around list
            aroundList.Clear();

            List<Tile> tempAround = _board.GetTilesAround(currentTile);

            if (tempAround == null) continue;

            foreach (Tile t in tempAround)
            {
                AddAllowedTile(t, allowedTiles);
            }

            // Calculation loop (f, g & h)
            foreach (Tile t in aroundList)
            {
                t.parent = currentTile;
                t.cost = t.parent.cost + 1;

                // Add it in open list
                if (!openList.Contains(t))
                {
                    openList.Add(t);
                }

                // Border tiles
                if (t.cost == distance)
                {
                    closedList.Add(t);
                    openList.Remove(t);
                }
            }
        }

        closedList.Remove(startTile);

        if (Utils.IsVoidList(closedList)) return null;

        return closedList;
    }

    /// <summary>
    /// Return ALL the tiles around a tile (with a defined distance).
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<Tile> AroundTiles(Tile startTile, int distance, bool withDiagonals = false)
    {
        ClearPath();

        if (distance == 0) return null; // EXIT : No distance, no path

        // Set first tile
        currentTile = startTile;
        currentTile.cost = 0;
        openList.Add(currentTile);

        // Tile search loop
        while (openList.Count > 0)
        {
            if (openList.Count > 0) // Get first tile in open list
            {
                currentTile = openList[0];
                closedList.Add(currentTile);
                openList.Remove(currentTile);
            }
            else // Open list is void -> all direction founded
            {
                closedList.Remove(startTile);

                if (Utils.IsVoidList(closedList)) return null; // EXIT : Nothing in closed list

                return closedList;
            }

            // Put all tiles around in around list
            aroundList.Clear();

            List<Tile> tempAround = _board.GetTilesAround(currentTile, !withDiagonals);

            if (tempAround == null) continue; // CONTINUE : No tiles around

            foreach (Tile t in tempAround)
            {
                AddAnyTile(t);
            }

            // Calculation loop (f, g & h)
            foreach (Tile t in aroundList)
            {
                t.parent = currentTile;
                t.cost = t.parent.cost + 1;

                // Add it in open list
                if (!openList.Contains(t))
                {
                    openList.Add(t);
                }

                // Border tiles
                if (t.cost == distance)
                {
                    closedList.Add(t);
                    openList.Remove(t);
                }
            }
        }

        closedList.Remove(startTile);

        if (Utils.IsVoidList(closedList)) return null;

        return closedList;
    }

    /// <summary>
    /// Reset the tiles pathfinding values (cost, heuristic, etc.) and clear the pathfinding lists.
    /// </summary>
    public void ClearPath()
    {
        openList
            .ForEach(tile => tile.ResetTileValues());
        closedList
            .ForEach(tile => tile.ResetTileValues());

        openList.Clear();
        closedList.Clear();
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
    /// Return true if a tile is in diagonal with another tile.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    private bool IsDiagonal(Tile tile1, Tile tile2)
    {
        if (tile1.x == tile2.x || tile1.y == tile2.y) return false;

        return true;
    }

    /// <summary>
    /// Return the cost of movement from tile to another.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="currentTile"></param>
    /// <returns></returns>
    private int GetCost(Tile tile, Tile currentTile)
    {
        if (IsDiagonal(currentTile, tile)) return 14;

        return 10;
    }

    /// <summary>
    /// Add tiles in around list excepts : hole, big obstacle, other character with conditions
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="lastTile"></param>
    private void AddAllowedTile(Tile tile, List<TileType> allowedTiles, Tile lastTile = null)
    {
        if (!tile) return; // EXIT : There is no tile
        if (closedList.Contains(tile)) return; // EXIT : This tile is in the closed list
        if (!allowedTiles.Contains(tile.type)) return; // EXIT : It's a forbiddent tile type
        if(tile.cost > 0)
        {
            int newCost = currentTile.cost + GetCost(tile, currentTile);
            if (newCost > tile.cost) return; // EXIT : New cost is higher than the previous (il is calculated)
        }

        if (tile.IsOccupiedByCharacter())
        {
            if (_rules.canPassThrough == M_Rules.PassThrough.Nobody)
            {
                if (tile != lastTile) return; // EXIT : Tile occupied by a character
            }
            else if (_rules.canPassThrough == M_Rules.PassThrough.AlliesOnly)
            {
                if (tile.Character().Team() != _characters.current.Team()) return; // EXIT : Tile occupied by an enemy
            }
        }

        aroundList.Add(tile);
    }

    /// <summary>
    /// Add all tiles in around list
    /// </summary>
    /// <param name="tile"></param>
    private void AddAnyTile(Tile tile)
    {
        if (!tile) return; // if tile
        if (closedList.Contains(tile)) return; // isnt this tile in list
        if(tile.cost > 0) // new cost is lower than current (if already calculated)
        {
            int newCost = currentTile.cost + GetCost(tile, currentTile);
            if (newCost > tile.cost) return;
        }

        aroundList.Add(tile);
    }

    /// <summary>
    /// Caluclate cost, heuristic and total of a tile and add its parent
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="endTile"></param>
    private void CalculateTileValues(Tile tile, Tile endTile)
    {
        if(tile == null) { Debug.LogError("tile is null !"); return; } // EXIT ERROR : Tile is null
        if(endTile == null) { Debug.LogError("end tile is null !"); return; } // EXIT ERROR : End tile is null
            
        // Calculate g, h and f
        tile.parent = currentTile;

        tile.cost = tile.parent.cost + GetCost(tile, currentTile);
        tile.heuristic = (Mathf.Abs(endTile.x - tile.x) + Mathf.Abs(endTile.y - tile.y)) * 10;
        tile.f = tile.cost + tile.heuristic;
        //tile.ShowValues();

        // Add it in open list
        if (openList.Contains(tile)) return;

        openList.Add(tile);
    }
}