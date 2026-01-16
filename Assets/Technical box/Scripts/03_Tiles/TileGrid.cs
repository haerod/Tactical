using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileGrid
{
    [SerializeField] private List<Tile> tiles;
    [SerializeField] private Tile[,] grid; // Note : Let it serializable to be dirty.

    public int lowestX { get; private set; }
    public int highestX { get; private set; }
    public int lowestY { get; private set; }
    public int highestY { get; private set; }

    public void Setup(List<Tile> tilesToAdd)
    {
        if (tilesToAdd.Count == 0)
        {
            Debug.LogError("No tiles on the board.");
            return; // No tiles to build the board
        }

        // Note : don't do this during editor time
        foreach (Tile tile in tilesToAdd)
        {
            if (tile.coordinates.x < lowestX)
                lowestX = tile.coordinates.x;
            if (tile.coordinates.y < lowestY)
                lowestY = tile.coordinates.y;
            if (tile.coordinates.x > highestX)
                highestX = tile.coordinates.x;
            if (tile.coordinates.y > highestY)
                highestY = tile.coordinates.y;
        }

        int width = Mathf.Abs(lowestX - highestX) + 1;
        int height = Mathf.Abs(lowestY - highestY) + 1;

        grid = new Tile[width, height];

        foreach (Tile tile in tilesToAdd)
        {
            grid[tile.coordinates.x - lowestX, tile.coordinates.y - lowestY] = tile;
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
}
