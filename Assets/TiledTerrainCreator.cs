using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledTerrainCreator : MonoBehaviour
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

    [HideInInspector] public TileStat[,] grid;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
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
        grid = new TileStat[length, width];
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 pozTile = new Vector3(i, 0, j);
                GameObject instaTile = Instantiate(tile, pozTile, Quaternion.identity, terrainParent);
                instaTile.name = "tile " + i + ", " + j;

                TileStat stat = instaTile.GetComponent<TileStat>();
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