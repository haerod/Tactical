using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EdgeGrid
{
    [SerializeField] private List<Edge> edgesToAdd; // Note : Let it serializable to be dirty.
    [SerializeField] private Edge[,] grid; // Note : Let it serializable to be dirty.

    public int lowestX { get; private set; }
    public int higherX { get; private set; }
    public int lowestY { get; private set; }
    public int higherY { get; private set; }

    public void Setup()
    {
        if (edgesToAdd.Count == 0)
        {
            return; // No tiles to build the board
        }

        // Note : don't do this during editor time
        foreach (Edge edge in edgesToAdd)
        {
            if (edge.coordinates.x < lowestX)
                lowestX = edge.coordinates.x;
            if (edge.coordinates.y < lowestY)
                lowestY = edge.coordinates.y;
            if (edge.coordinates.x > higherX)
                higherX = edge.coordinates.x;
            if (edge.coordinates.y > higherY)
                higherY = edge.coordinates.y;
        }

        int width = Mathf.Abs(lowestX - higherX) + 1;
        int height = Mathf.Abs(lowestY - higherY) + 1;

        grid = new Edge[width, height];

        foreach (Edge edge in edgesToAdd)
        {
            grid[edge.coordinates.x - lowestX, edge.coordinates.y - lowestY] = edge;
        }
    }

    // IMPLICIT CAST
    public Edge this[int x, int y]
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
    public void AddEdge(Edge edge) => edgesToAdd.AddIfNew(edge);
    public void RemoveEdge(Edge edge) => edgesToAdd.Remove(edge);
}
