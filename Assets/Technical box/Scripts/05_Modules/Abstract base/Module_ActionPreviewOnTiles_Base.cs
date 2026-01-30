using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

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

    private void Start()
    {
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        GameEvents.OnAnyActionStart += Action_OnAnyActionStart;
        _Level.OnVictory += Level_OnVictory;
    }

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
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        HideFeedbacks();
    }
    
    private void Level_OnVictory(object sender, EventArgs e)
    {
        HideFeedbacks();
    }
}
