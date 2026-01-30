using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Preview of the Attack action on tiles.
/// </summary>
public class Module_ActionPreviewOnTiles_Heal : Module_ActionPreviewOnTiles_Base
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
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
        if (!startingUnit.actionsHolder.HasAvailableAction<A_Heal>()) 
            return; // No usable heal action
        
        ShowFeedbacks(startingUnit.actionsHolder.GetActionOfType<A_Heal>().healableUnits
            .Select(unit => unit.tile)
            .ToList());
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        HideFeedbacks();
    }
    
    private void Action_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if (!endingActionUnit.behavior.playable) 
            return; // NPC
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if (!endingActionUnit.actionsHolder.HasAvailableAction<A_Heal>()) 
            return; // No usable heal action
        
        ShowFeedbacks(endingActionUnit.actionsHolder.GetActionOfType<A_Heal>().healableUnits
            .Select(unit => unit.tile)
            .ToList());    
    }
}
