using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Pathfinding : MonoSingleton<M_Pathfinding>
{
    private List<Tile> openList = new List<Tile>();
    private List<Tile> closedList = new List<Tile>();
    private List<Tile> aroundList = new List<Tile>();
    private Tile currentTile;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // Returns the path, including end tile
    // If no path, returns null
    public List<Tile> Pathfind(Tile startTile, Tile endTile)
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
            List<Tile> tempAround = GetAroundTiles(currentTile);

            if (tempAround == null) continue;

            foreach (Tile t in tempAround)
            {
                AddTile(t, endTile);
            }

            // If it's end tile -> return path
            if (aroundList.Contains(endTile))
            {
                closedList.Add(endTile);
                List<Tile> toReturn = new List<Tile>();
                endTile.parent = currentTile;
                toReturn.Add(endTile);
                while (endTile != startTile)
                {
                    Tile t = endTile.parent;
                    endTile = t;
                    toReturn.Add(endTile);
                }

                toReturn.Reverse();
                return toReturn;
            }

            // Calculation loop (f, g & h)
            foreach (Tile tile in aroundList)
            {
                CalculateTileValues(tile, endTile);
            }
        }

        return null;
    }

    public Tile ClosestFreeTileWithShortestPath(Tile from, Tile to, int distance = 10)
    {
        if (from == null || to == null) return null; // EXIT : from or to doen't exist
        if (from == to) return null; // EXIT : from == to
        if (distance == 0) return null; // EXIT : no distance


        List<Tile> area = AreaMovementZone(to, distance).ToList();

        // Get aviable tiles
        area = area
            .Where(o => !o.IsOccupied() || o == from) // remove occupied tiles or get the original tile
            .Where(o => o.type != Tile.Type.Hole || o.type != Tile.Type.BigObstacle) // remove occupied tiles
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
            List<Tile> testedPath = Pathfind(from, tile);

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

    public List<Tile> AreaMovementZone(Tile startTile, int distance)
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

            List<Tile> tempAround = GetAroundTiles(currentTile);

            if (tempAround == null) continue;

            foreach (Tile t in tempAround)
            {
                AddTile(t);
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

    public List<Tile> AroundTiles(Tile startTile, int distance)
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

            List<Tile> tempAround = GetAroundTiles(currentTile, true);

            if (tempAround == null) continue;

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

    public void ClearPath()
    {
        foreach (Tile t in openList)
        {
            t.ResetTileValues();
        }
        openList.Clear();
        foreach (Tile t in closedList)
        {
            t.ResetTileValues();
        }
        closedList.Clear();
    }

    public enum LoSParameters { WithStart, WithEnd, WithStartAndEnd, WithoutStartAndEnd }
    public List<Tile> LineOfSight(Tile startTile, Tile endTile, LoSParameters parameters = LoSParameters.WithoutStartAndEnd)
    {
        // Get Vector between start and end coordinates (Start tile - end tile)
        Vector2 v2 = new Vector2(endTile.x - startTile.x, endTile.y - startTile.y);

        // Get the length of distance
        float length = Mathf.Max(Mathf.Abs(v2.x), Mathf.Abs(v2.y));

        List<Tile> toReturn = new List<Tile>();

        if(parameters == LoSParameters.WithStart || parameters == LoSParameters.WithStartAndEnd)
        {
            toReturn.Add(startTile);
        }

        for (int i = 1; i < length+1; i++)
        {
            // Theoric coordinates of the segment
            Vector2 tile = new Vector2(startTile.x, startTile.y) + i / length * v2;
            Tile t = _terrain.GetTile(Mathf.RoundToInt(tile.x), Mathf.RoundToInt(tile.y));
            toReturn.Add(t);
        }

        if (parameters == LoSParameters.WithStart || parameters == LoSParameters.WithoutStartAndEnd)
        {
            toReturn.Remove(endTile);
        }

        return toReturn; 
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // Get a tile with an offset, if it exits in terrain (holes included)
    private Tile GetOffsetTile(int xOffset, int yOffset, Tile tile)
    {
        if (!InTerrainRange(xOffset, yOffset, tile)) return null;

        return _terrain.grid[tile.x + xOffset, tile.y + yOffset];
    }

    // Get all tiles around
    private List<Tile> GetAroundTiles(Tile tile, bool forceNoDiagonals = false)
    {
        List<Tile> toReturn = new List<Tile>();
        bool useDiagonals = _rules.useDiagonals;

        if (forceNoDiagonals) // Force no diagonals (aim zone for exemple)
            useDiagonals = false;

        toReturn.Add(GetOffsetTile(0, -1, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(-1, -1, tile));

        toReturn.Add(GetOffsetTile(-1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(-1, 1, tile));

        toReturn.Add(GetOffsetTile(0, 1, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(1, 1, tile));

        toReturn.Add(GetOffsetTile(1, 0, tile));

        if (useDiagonals)
            toReturn.Add(GetOffsetTile(1, -1, tile));

        if (Utils.IsVoidList(toReturn)) return null;
        
        return toReturn;
    }

    // Is tile in diagonal with other tile
    private bool IsDiagonal(Tile tile1, Tile tile2)
    {
        if (tile1.x == tile2.x || tile1.y == tile2.y) return false;

        return true;
    }

    // Get cost of movement from tile to another
    private int GetCost(Tile tile, Tile currentTile)
    {
        if (IsDiagonal(currentTile, tile)) return 14;

        return 10;
    }

    // Is tile in terrain
    private bool InTerrainRange(int xOffset, int yOffset, Tile tile)
    {
        if (tile.x + xOffset < 0) return false;
        if (tile.x + xOffset >= _terrain.grid.GetLength(0)) return false;
        if (tile.y + yOffset < 0) return false;
        if (tile.y + yOffset >= _terrain.grid.GetLength(1)) return false;

        return true;
    }

    // Add tiles in around list if the have conditions (Without : hole, big obstacle, other character with conditions)
    private void AddTile(Tile tile, Tile lastTile = null)
    {
        if (!tile) return; // if tile
        if (closedList.Contains(tile)) return; // isnt this tile in list
        if (tile.type == Tile.Type.Hole) return; // isnt a hole
        if (tile.type == Tile.Type.BigObstacle) return; // isnt a big obstacle
        if(tile.cost > 0) // new cost is lower than current (if already calculated)
        {
            int newCost = currentTile.cost + GetCost(tile, currentTile);
            if (newCost > tile.cost) return;
        }

        if (tile.IsOccupied())
        {
            if (_rules.canPassAcross == M_GameRules.PassAcross.Nobody)
            {
                if (tile != lastTile) return;
            }
            else if (_rules.canPassAcross == M_GameRules.PassAcross.AlliesOnly)
            {
                if (tile.Character().Team() != _characters.currentCharacter.Team()) return;
            }
        }

        aroundList.Add(tile);
    }

    // Add tiles in around list if the have conditions
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

    // Caluclate cost, heuristic and total
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