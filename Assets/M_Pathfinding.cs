﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Pathfinding : MonoSingleton<M_Pathfinding>
{
    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private List<Tile> openList = new List<Tile>();
    private List<Tile> closedList = new List<Tile>();
    private List<Tile> aroundList = new List<Tile>();
    private Tile currentTile;

    private Tile[,] grid;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // Returns the path
    // If no path, returns null
    // NOTE
    // LE PATHFINING POURRAIT ETRE AMELIORE
    // SI LORSQU'ON VERIFIE SI LE NOUVEAU COUT EST INFERIEUR, ET QUE CE N'EST PAS LE CAS
    // ALORS IL FAUDRAIT VERIFIER AVEC LE GRAND PARENT SI CE N'EST PAS PLUS COURT
    public List<Tile> Pathfind(Tile startTile, Tile endTile, bool passAcrossEndTile = false)
    {
        ClearPath();

        // Set bacis values
        startX = startTile.x;
        startY = startTile.y;
        endX = endTile.x;
        endY = endTile.y;

        grid = _terrain.grid;

        // Set first tile
        currentTile = grid[startX, startY];
        currentTile.cost = 0;
        openList.Add(currentTile);
        endTile = grid[endX, endY];

        // Pathfinding loop
        while (openList.Count > 0)
        {            
            if (openList.Count > 0) // Choose current tile in open list
            {
                currentTile = openList.OrderBy(o => o.f).FirstOrDefault();
                closedList.Add(currentTile);
                openList.Remove(currentTile);
            }
            else // Open list is void -> no direction in the path, return null
            {
                NoDirectionEnd();
                return null;
            }

            // Put all tiles in around list
            aroundList.Clear();
            List<Tile> tempAround = GetAroundTiles(currentTile);

            if (tempAround == null) continue;

            foreach (Tile t in tempAround)
            {
                AddTile(t, passAcrossEndTile, endTile);
            }

            // If it's end tile -> return path
            if (aroundList.Contains(endTile))
            {
                closedList.Add(endTile);
                List<Tile> toReturn = new List<Tile>();
                endTile.parent = currentTile;
                toReturn.Add(endTile);
                while (endTile != grid[startX, startY])
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
                CalculateTileValues(tile);
            }
        }

        return null;
    }

    public List<Tile> PathfindAround(Tile startTile, Tile endTile, bool passAcrossEndTile)
    {
        List<Tile> toReturn = new List<Tile>();
        toReturn = Pathfind(startTile, endTile, passAcrossEndTile);

        if (Utils.IsVoidList(toReturn)) return null;

        toReturn.ToList();
        toReturn.Remove(toReturn.Last());

        if (Utils.IsVoidList(toReturn)) return null;        

        return toReturn;
    }

    public List<Tile> AreaMovementZone(Tile tile, int distance)
    {
        ClearPath();

        if (distance == 0)
        {
            return closedList;
        }

        // Set bacis values
        startX = tile.x;
        startY = tile.y;

        grid = _terrain.grid;

        // Set first tile
        currentTile = grid[startX, startY];
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
                closedList.Remove(grid[startX, startY]);
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

        closedList.Remove(grid[startX, startY]);

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

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private Tile GetOffsetTile(int xOffset, int yOffset, Tile tile)
    {
        if (!InTerrainRange(xOffset, yOffset, tile)) return null;

        return grid[tile.x + xOffset, tile.y + yOffset];
    }

    // Get all tiles around
    private List<Tile> GetAroundTiles(Tile tile)
    {
        List<Tile> toReturn = new List<Tile>();
        bool useDiagonals = _rules.useDiagonals;

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

    private bool IsDiagonal(Tile tile1, Tile tile2)
    {
        if (tile1.x == tile2.x || tile1.y == tile2.y) return false;

        return true;
    }

    private int GetCost(Tile tile, Tile currentTile)
    {
        if (IsDiagonal(currentTile, tile)) return 14;

        return 10;
    }

    private bool InTerrainRange(int xOffset, int yOffset, Tile tile)
    {
        if (tile.x + xOffset < 0) return false;
        if (tile.x + xOffset >= grid.GetLength(0)) return false;
        if (tile.y + yOffset < 0) return false;
        if (tile.y + yOffset >= grid.GetLength(1)) return false;

        return true;
    }

    private void NoDirectionEnd()
    {
        Debug.Log("No direction end");
    }

    private void AddTile(Tile tile, bool passAcrossLastTile = false, Tile lastTile = null)
    {
        if (!tile) return; // if tile
        if (closedList.Contains(tile)) return; // isnt this tile in list
        if (tile.hole) return; // isnt a hole
        if(tile.cost > 0) // new cost is lower than current (if already calculated)
        {
            int newCost = currentTile.cost + GetCost(tile, currentTile);
            if (newCost > tile.cost) return;
        }

        if (tile.IsOccupied())
        {
            switch (_rules.canPassAcross)
            {
                case M_GameRules.PassAcross.Everybody:
                    break;
                case M_GameRules.PassAcross.Nobody:
                    if (!passAcrossLastTile) return;
                    if (tile != lastTile) return;

                    break;

                default:
                    break;
            }
        }

        aroundList.Add(tile);
    }

    private void CalculateTileValues(Tile tile)
    {
        // Calculate g, h and f
        tile.parent = currentTile;

        tile.cost = tile.parent.cost + GetCost(tile, currentTile);
        tile.heuristic = (Mathf.Abs(endX - tile.x) + Mathf.Abs(endY - tile.y)) * 10;
        tile.f = tile.cost + tile.heuristic;
        //tile.ShowValues();

        // Add it in open list
        if (openList.Contains(tile)) return;

        openList.Add(tile);
    }
}
