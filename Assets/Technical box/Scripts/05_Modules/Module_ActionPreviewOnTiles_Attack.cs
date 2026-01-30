using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Preview of the Attack action on tiles.
/// </summary>
public class Module_ActionPreviewOnTiles_Attack : Module_ActionPreviewOnTiles_Base
{
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
    }
    
    private void OnDisable()
    {
        if(currentUnit)
            currentUnit.weaponHolder.OnWeaponChange -= WeaponsHolder_OnWeaponChanged;
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
        if (!startingUnit.actionsHolder.HasAvailableAction<A_Attack>()) 
            return; // No usable attack action
        
        currentUnit = startingUnit;
        currentUnit.weaponHolder.OnWeaponChange += WeaponsHolder_OnWeaponChanged;
        ShowFeedbacks(startingUnit.attack.AttackableTiles());
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        if (!endingUnit.actionsHolder.HasAvailableAction<A_Attack>()) 
            return; // No usable attack action
        
        currentUnit.weaponHolder.OnWeaponChange -= WeaponsHolder_OnWeaponChanged;
        HideFeedbacks();
        
        currentUnit = null;
    }
    
    private void Action_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if (!endingActionUnit.behavior.playable) 
            return; // NPC
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if (!endingActionUnit.actionsHolder.HasAvailableAction<A_Attack>()) 
            return; // No attack action
        
        ShowFeedbacks(endingActionUnit.attack.AttackableTiles());
    }
    
    private void WeaponsHolder_OnWeaponChanged(object sender, Unit_WeaponHolder.WeaponChangeEventArgs args)
    {
        HideFeedbacks();
        ShowFeedbacks(_units.current.attack.AttackableTiles());
    }
}
