using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Preview of the Move action on tiles.
/// </summary>
public class Module_UnitMovementPreview : MonoBehaviour
{
    [SerializeField] private float feedbackYOffset;
    
    [Header("- REFERENCES -")]
    
    [SerializeField] private GameObject tileAreaPrefab;
    
    private List<GameObject> areas = new();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnUnitExit += InputEvents_OnUnitExit;
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
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
    private void ShowFeedbacks(List<Tile> tilesToShow)
    {
        HideFeedbacks();
        
        foreach (Tile tile in tilesToShow)
        {
            GameObject instantiatedTile = Instantiate(tileAreaPrefab, transform);
            instantiatedTile.transform.position = new Vector3(
                tile.transform.position.x,
                instantiatedTile.transform.position.y + feedbackYOffset,
                tile.transform.position.z);
            
            areas.Add(instantiatedTile);
        }
    }

    /// <summary>
    /// Resets the tiles skin and clears the movement area tiles list.
    /// </summary>
    private void HideFeedbacks()
    {
        areas.ForEach(area  => Destroy(area.gameObject));
        areas.Clear();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void InputEvents_OnUnitEnter(object sender, Unit enteredUnit)
    {
        if(enteredUnit == _units.current)
            return; // Current unit
        
        ShowFeedbacks(enteredUnit.move.movementArea);
    }
    
    private void InputEvents_OnUnitExit(object sender, Unit e)
    {
        HideFeedbacks();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        HideFeedbacks();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        HideFeedbacks();
    }
}