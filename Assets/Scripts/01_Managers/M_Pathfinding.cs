﻿using UnityEngine;
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

    public enum LoSParameters { WithStart, WithEnd, WithStartAndEnd, WithoutStartAndEnd} // Options of the line of sight

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
                AddAllowedTile(t, endTile);
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

    /// <summary>
    /// Return the closest free tile from a target to its destination (with a maximum distance of this destination).
    /// Ex : a character wants the closest free tile around its target.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Tile ClosestFreeTileWithShortestPath(Tile from, Tile to, int distance = 10)
    {
        if (from == null || to == null) return null; // EXIT : from or to doen't exist
        if (from == to) return null; // EXIT : from == to
        if (distance == 0) return null; // EXIT : no distance

        List<Tile> area = AreaMovementZone(to, distance).ToList();

        // Get aviable tiles
        area = area
            .Where(o => !o.IsOccupiedByCharacter() || o == from) // remove occupied tiles or get the original tile
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

    /// <summary>
    /// Return the tiles around a tile (with a defined distance), considering holes and obstacles as blocking tiles.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
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
                AddAllowedTile(t);
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
    public List<Tile> AroundTiles(Tile startTile, int distance)
    {
        ClearPath();

        if (distance == 0)
        {
            return null;
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

    /// <summary>
    /// Reset the tiles pathfinding values (cost, heuristic, etc.) and clear the pathfinding lists.
    /// </summary>
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

    /// <summary>
    /// Return a line of sight from a tile to another one.
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Return a tile with an offset, if it exits in the board (holes included).
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private Tile GetOffsetTile(int xOffset, int yOffset, Tile tile)
    {
        if (!InBoardRange(xOffset, yOffset, tile)) return null;

        return _terrain.grid[tile.x + xOffset, tile.y + yOffset];
    }

    /// <summary>
    /// Return all the existing tiles around a tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="forceNoDiagonals"></param>
    /// <returns></returns>
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
    /// Return true if a tile is inside the board range.
    /// </summary>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool InBoardRange(int xOffset, int yOffset, Tile tile)
    {
        if (tile.x + xOffset < 0) return false;
        if (tile.x + xOffset >= _terrain.grid.GetLength(0)) return false;
        if (tile.y + yOffset < 0) return false;
        if (tile.y + yOffset >= _terrain.grid.GetLength(1)) return false;

        return true;
    }

    /// <summary>
    /// Add tiles in around list excepts : hole, big obstacle, other character with conditions
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="lastTile"></param>
    private void AddAllowedTile(Tile tile, Tile lastTile = null)
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

        if (tile.IsOccupiedByCharacter())
        {
            if (_rules.canPassThrough == M_Rules.PassThrough.Nobody)
            {
                if (tile != lastTile) return;
            }
            else if (_rules.canPassThrough == M_Rules.PassThrough.AlliesOnly)
            {
                if (tile.Character().Team() != _characters.currentCharacter.Team()) return;
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