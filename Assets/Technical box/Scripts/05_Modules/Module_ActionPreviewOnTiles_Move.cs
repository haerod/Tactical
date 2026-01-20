using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Preview of the Move action on tiles.
/// </summary>
public class Module_ActionPreviewOnTiles_Move : Module_ActionPreviewOnTiles_Base
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionStart += Action_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
        _rules.OnVictory += Rules_OnVictory;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if (!startingUnit.behavior.playable) 
            return; // NPC
        if(!startingUnit.CanPlay())
            return; // Can't play
        if (!startingUnit.actions.HasUsableAction<A_Move>())
            return; // No move action
        
        ShowFeedbacks(startingUnit.move.movementArea);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionEnd(object sender, Unit endingActionUnit)
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