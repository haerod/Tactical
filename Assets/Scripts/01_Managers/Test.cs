using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test instance;

    public Tile tile1, tile2;
    public List<TileType> allowedTileTypes;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
    }
}
