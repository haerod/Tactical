using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Terrain : MonoSingleton<M_Terrain>
{
    [Header("TERRAIN PARAMETERS")]
    [Range(1, 100)]
    public int length = 5;
    [Range(1, 100)]
    public int width = 5;

    [Header("SEPCIAL TILES")]
    public List<Vector2Int> holeCoordinates;

    [Header("REFERENCES")]

    [SerializeField] private GameObject tile = null;
    [SerializeField] private Transform terrainParent = null;

    [HideInInspector] public Tile[,] grid;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // Replace Awake
    protected override void Init()
    {
        GenerateTerrain();        
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void GenerateTerrain()
    {
        DestroyTerrain();
        CreateTiles();
    }

    public Tile GetTile(int x, int y)
    {
        return grid[x, y];
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void DestroyTerrain()
    {
        if (terrainParent.childCount == 0) return;

        foreach(Transform child in terrainParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateTiles()
    {
        grid = new Tile[length, width];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 pozTile = new Vector3(i, 0, j);
                GameObject instaTile = Instantiate(tile, pozTile, Quaternion.identity, terrainParent);
                instaTile.name = "tile " + i + ", " + j;

                Tile stat = instaTile.GetComponent<Tile>();
                stat.x = i;
                stat.y = j;
                grid[i, j] = stat;

                Vector2Int hole = new Vector2Int(i, j);
                if (holeCoordinates.Contains(hole))
                {
                    stat.hole = true;
                    stat.DisableRenderer();
                    stat.HideValues();
                }
            }
        }
    }
}