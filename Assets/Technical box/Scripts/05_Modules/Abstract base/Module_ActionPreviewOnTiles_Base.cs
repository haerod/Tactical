using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class Module_ActionPreviewOnTiles_Base : MonoBehaviour
{
    [SerializeField] private Material feedbackTileMaterial;

    [Header("REFERENCES")]
    
    [SerializeField] private GameObject tileAreaPrefab;
    
    private List<GameObject> areas = new();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Shows the tiles of the movement area.
    /// </summary>
    protected void ShowFeedbacks(List<Tile> tilesToShow)
    {
        foreach (Tile tile in tilesToShow)
        {
            GameObject instantiatedTile = Instantiate(tileAreaPrefab, transform);
            instantiatedTile.transform.position = new Vector3(
                tile.transform.position.x,
                instantiatedTile.transform.position.y,
                tile.transform.position.z);
            instantiatedTile.GetComponent<Renderer>().material = feedbackTileMaterial;
            areas.Add(instantiatedTile);
        }
    }

    /// <summary>
    /// Resets the tiles skin and clears the movement area tiles list.
    /// </summary>
    protected void HideFeedbacks()
    {
        areas.ForEach(area  => Destroy(area.gameObject));
        areas.Clear();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}
