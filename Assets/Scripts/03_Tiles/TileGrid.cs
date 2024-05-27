using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileGrid
{
    [SerializeField] private List<Tile> tilesToAdd; // Note : Let it serializable to be dirty.
    [SerializeField] private Tile[,] grid; // Note : Let it serializable to be dirty.

    public int lowestX { get; private set; }
    public int higherX { get; private set; }
    public int lowestY { get; private set; }
    public int higherY { get; private set; }

    public void Setup()
    {
        if (tilesToAdd.Count == 0)
        {
            Debug.LogError("No tiles on the board.");
            return; // No tiles to build the board
        }

        // Note : don't do this during editor time
        foreach (Tile tile in tilesToAdd)
        {
            if (tile.x < lowestX)
                lowestX = tile.x;
            if (tile.y < lowestY)
                lowestY = tile.y;
            if (tile.x > higherX)
                higherX = tile.x;
            if (tile.y > higherY)
                higherY = tile.y;
        }

        int width = Mathf.Abs(lowestX - higherX) + 1;
        int height = Mathf.Abs(lowestY - higherY) + 1;

        grid = new Tile[width, height];

        foreach (Tile tile in tilesToAdd)
        {
            grid[tile.x - lowestX, tile.y - lowestY] = tile;
        }
    }


    // IMPLICIT CAST
    public Tile this[int x, int y]
    {
        get
        {
            if (!grid.IsInBounds(x - lowestX, y - lowestY))
                return null;

            return grid[x - lowestX, y - lowestY];
        }
        private set { }
    }

    // SETTERS
    public void AddTile(Tile tile) => tilesToAdd.AddIfNew(tile);
    public void RemoveTile(Tile tile) => tilesToAdd.Remove(tile);
}
