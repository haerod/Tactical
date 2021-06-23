using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class M_Pathfinding : MonoBehaviour
{
    public static M_Pathfinding inst;

    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private List<Tile> openList = new List<Tile>();
    private List<Tile> closedList = new List<Tile>();
    private List<Tile> aroundList = new List<Tile>();
    private Tile currentTile;

    private Tile[,] grid;
    private Tile endTile;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // Returns the path
    // If no path, returns null
    // NOTE
    // LE PATHFINING POURRAIT ETRE AMELIORE
    // SI LORSQU'ON VERIFIE SI LE NOUVEAU COUT EST INFERIEUR, ET QUE CE N'EST PAS LE CAS
    // ALORS IL FAUDRAIT VERIFIER AVEC LE GRAND PARENT SI CE N'EST PAS PLUS COURT
    public List<Tile> Pathfind(int sX, int sY, int eX, int eY)
    {
        ClearPath();

        // Set bacis values
        startX = sX;
        startY = sY;
        endX = eX;
        endY = eY;

        grid = M_Terrain.inst.grid;

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

            bool useDiagonals = M_GameRules.inst.useDiagonals;

            AddTile(GetAroundCurrentTile(0, -1));
            if(useDiagonals) AddTile
                    (GetAroundCurrentTile(-1, -1));
            AddTile(GetAroundCurrentTile(-1, 0));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(-1, 1));
            AddTile(GetAroundCurrentTile(0, 1));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(1, 1));
            AddTile(GetAroundCurrentTile(1, 0));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(1, -1));

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

    public List<Tile> AreaMovementZone(int sX, int sY, int distance)
    {
        ClearPath();

        if (distance == 0)
        {
            return closedList;
        }

        // Set bacis values
        startX = sX;
        startY = sY;

        grid = M_Terrain.inst.grid;

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
            bool useDiagonals = M_GameRules.inst.useDiagonals;

            AddTile(GetAroundCurrentTile(0, -1));
            if (useDiagonals) AddTile
                     (GetAroundCurrentTile(-1, -1));
            AddTile(GetAroundCurrentTile(-1, 0));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(-1, 1));
            AddTile(GetAroundCurrentTile(0, 1));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(1, 1));
            AddTile(GetAroundCurrentTile(1, 0));
            if (useDiagonals)
                AddTile(GetAroundCurrentTile(1, -1));

            // Calculation loop (f, g & h)
            foreach (Tile tile in aroundList)
            {
                tile.parent = currentTile;
                tile.cost = tile.parent.cost + 1;

                // Add it in open list
                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }

                // Border tiles
                if (tile.cost == distance)
                {
                    closedList.Add(tile);
                    openList.Remove(tile);
                }
            }
        }

        closedList.Remove(grid[startX, startY]);
        return closedList;
    }

    public List<Tile> PathfindAround(int sX, int sY, int eX, int eY)
    {
        List<Tile> tilesList = new List<Tile>();
        Tile tileToCheck = M_Terrain.inst.grid[eX, eY];

        bool useDiagonals = M_GameRules.inst.useDiagonals;

        // Add to tiles list
        tilesList.Add(GetAround(0, -1, tileToCheck));
        if (useDiagonals)
            tilesList.Add(GetAround(-1, -1, tileToCheck));
        tilesList.Add(GetAround(-1, 0, tileToCheck));
        if (useDiagonals)
            tilesList.Add(GetAround(-1, 1, tileToCheck));
        tilesList.Add(GetAround(0, 1, tileToCheck));
        if (useDiagonals)
            tilesList.Add(GetAround(1, 1, tileToCheck));
        tilesList.Add(GetAround(1, 0, tileToCheck));
        if (useDiagonals)
            tilesList.Add(GetAround(1, -1, tileToCheck));

        List<Tile> temp = tilesList.ToList(); // LINQ

        // Remove holes and null
        foreach (Tile t in temp) 
        {
            if (t == null)
            {
                tilesList.Remove(t);
                continue;
            }

            if (!t.hole) continue;

            tilesList.Remove(t);
        }

        List<List<Tile>> pathes = new List<List<Tile>>();

        // Get all non-null pathes
        foreach (Tile t in tilesList)
        {
            List<Tile> current = Pathfind(sX, sY, t.x, t.y);

            if (current == null || current.Count == 0) continue;

            current = current.ToList();
            pathes.Add(current);
        }

        if (pathes == null || pathes.Count == 0) return null;

        return pathes
            .Where(pathCheck => pathCheck.Count == pathes.Select(e => e.Count).Min())
            .OrderBy(e => e.Select(t => t.cost).Sum()).FirstOrDefault().ToList();
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

        endTile = null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private Tile GetAroundCurrentTile(int x, int y)
    {
        if (!InTerrainRange(x, y, currentTile)) return null;

        return grid[currentTile.x + x, currentTile.y + y];
    }

    private Tile GetAround(int x, int y, Tile tile)
    {
        //print(x + ", " + y + " / " + tile.x + ", " + tile.y);
        if (!InTerrainRange(x, y, tile)) return null;

        return grid[tile.x + x, tile.y + y];
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

    private void AddTile(Tile tile)
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
            switch (M_GameRules.inst.canPassAcross)
            {
                case M_GameRules.PassAcross.Everybody:
                    break;
                case M_GameRules.PassAcross.Nobody:
                    return;
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
