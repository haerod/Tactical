using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Preview of the Move action on tiles.
/// </summary>
public class FM_TilesActionPreview_Move : MonoBehaviour
{
    [SerializeField] private Material feedbackTileMaterial;
    
    private List<Tile> tilesWithFeedback = new();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        A__Action.OnAnyActionStart += Action_OnAnyActionStart;
        A__Action.OnAnyActionEnd += Action_OnAnyActionEnd;
        _rules.OnVictory += Rules_OnVictory;
    }
    
    private void OnDisable()
    {
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        A__Action.OnAnyActionStart -= Action_OnAnyActionStart;
        A__Action.OnAnyActionEnd -= Action_OnAnyActionEnd;
        _rules.OnVictory -= Rules_OnVictory;
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
    private void ShowFeedbacks(List<Tile> tilesToShow) =>
        tilesToShow
            .ForEach(t =>
            {
                t.SetMaterial(feedbackTileMaterial);
                tilesWithFeedback.Add(t);
            });

    /// <summary>
    /// Resets the tiles skin and clears the movement area tiles list.
    /// </summary>
    private void HideFeedbacks()
    {
        tilesWithFeedback
            .ForEach(t => t.ResetTileSkin());

        tilesWithFeedback.Clear();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if (!startingUnit.behavior.playable) 
            return; // NPC
        if(!startingUnit.CanPlay())
            return; // Can't play
        if (!startingUnit.actions.HasUsableAction<A_Move>())
            return; // No move action
        
        ShowFeedbacks(startingUnit.move.movementArea);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionStart(object sender, U__Unit startingActionUnit)
    {
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionEnd(object sender, U__Unit endingActionUnit)
    {
        if (!endingActionUnit.behavior.playable) 
            return; // NPC
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if (!endingActionUnit.actions.HasUsableAction<A_Move>())
            return; // No move action
        
        ShowFeedbacks(endingActionUnit.move.movementArea);
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        HideFeedbacks();
    }
}