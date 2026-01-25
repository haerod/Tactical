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
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        GameEvents.OnAnyActionStart += Action_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
        _Level.OnVictory += Level_OnVictory;
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
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
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
        if (!endingActionUnit.actionsHolder.HasAvailableAction<A_Attack>()) 
            return; // No attack action
        
        ShowFeedbacks(endingActionUnit.attack.AttackableTiles());
    }
    
    private void WeaponsHolder_OnWeaponChanged(object sender, Unit_WeaponHolder.WeaponChangeEventArgs args)
    {
        HideFeedbacks();
        ShowFeedbacks(_units.current.attack.AttackableTiles());
    }
    
    private void Level_OnVictory(object sender, EventArgs e)
    {
        HideFeedbacks();
    }
}
