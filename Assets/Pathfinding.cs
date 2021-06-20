using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// NOTE
// LE PATHFINING POURRAIT ETRE AMELIORE
// SI LORSQU'ON VERIFIE SI LE NOUVEAU COUT EST INFERIEUR, ET QUE CE N'EST PAS LE CAS
// ALORS IL FAUDRAIT VERIFIER AVEC LE GRAND PARENT SI CE N'EST PAS PLUS COURT

public class Pathfinding : MonoBehaviour
{
    public TiledTerrainCreator terrainCreator;

    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private List<TileStat> openList = new List<TileStat>();
    private List<TileStat> closedList = new List<TileStat>();
    private List<TileStat> aroundList = new List<TileStat>();
    private TileStat currentTile;

    private TileStat[,] grid;
    private TileStat endTile;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // Returns the path
    // If no path, returns null
    public List<TileStat> Pathfind(int sX, int sY, int eX, int eY)
    {
        ClearPath();

        // Set bacis values
        startX = sX;
        startY = sY;
        endX = eX;
        endY = eY;

        grid = terrainCreator.grid;

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
                currentTile = GetLowest(); // LINQ
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
            AddTile(GetAround(0, 1));
            AddTile(GetAround(1, 1));
            AddTile(GetAround(1, 0));
            AddTile(GetAround(1, -1));
            AddTile(GetAround(0, -1));
            AddTile(GetAround(-1, -1));
            AddTile(GetAround(-1, 0));
            AddTile(GetAround(-1, 1));

            // If it's end tile -> return path
            if (aroundList.Contains(endTile))
            {
                closedList.Add(endTile);
                List<TileStat> toReturn = new List<TileStat>();
                endTile.parent = currentTile;
                toReturn.Add(endTile);
                while (endTile != grid[startX, startY])
                {
                    TileStat t = endTile.parent;
                    endTile = t;
                    toReturn.Add(endTile);
                }

                toReturn.Reverse();
                return toReturn;
            }

            // Calculation loop (f, g & h)
            foreach (TileStat tile in aroundList)
            {
                CalculateTileValues(tile);
            }
        }

        return null;
    }

    public void ClearPath()
    {
        foreach (TileStat t in openList)
        {
            t.ResetTile();
        }
        openList.Clear();
        foreach (TileStat t in closedList)
        {
            t.ResetTile();
        }
        closedList.Clear();

        endTile = null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private TileStat GetAround(int x, int y)
    {
        if (!InTerrainRange(x, y)) return null;

        return grid[currentTile.x + x, currentTile.y + y];
    }

    private TileStat GetLowest()
    {
        TileStat lowest = openList[0];
        for (int i = 0; i < openList.Count; i++)
        {
            if (openList[i].f > lowest.f) continue;
            lowest = openList[i];
        }
        return lowest;
    }

    private bool IsDiagonal(TileStat tile1, TileStat tile2)
    {
        if (tile1.x == tile2.x || tile1.y == tile2.y) return false;

        return true;
    }

    private int GetCost(TileStat tile, TileStat currentTile)
    {
        if (IsDiagonal(currentTile, tile)) return 14;

        return 10;
    }

    private bool InTerrainRange(int xOffset, int yOffset)
    {
        if (currentTile.x + xOffset < 0) return false;
        if (currentTile.x + xOffset >= grid.GetLength(0)) return false;
        if (currentTile.y + yOffset < 0) return false;
        if (currentTile.y + yOffset >= grid.GetLength(1)) return false;

        return true;
    }

    private void NoDirectionEnd()
    {
        Debug.Log("No direction end");
    }

    private void AddTile(TileStat tile)
    {
        if (!tile) return; // if tile
        if (closedList.Contains(tile)) return; // isnt this tile in list
        if (tile.hole) return; // isnt a hole
        if(tile.cost > 0) // new cost is lower than current (if already calculated)
        {
            int newCost = currentTile.cost + GetCost(tile, currentTile);
            if (newCost > tile.cost) return;
        }

        aroundList.Add(tile);
    }

    private void CalculateTileValues(TileStat tile)
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
