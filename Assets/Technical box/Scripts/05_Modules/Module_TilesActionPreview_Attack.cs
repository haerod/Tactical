using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Preview of the Attack action on tiles.
/// </summary>
public class Module_TilesActionPreview_Attack : MonoBehaviour
{
    [SerializeField] private Material feedbackTileMaterial;
    private List<Tile> tilesWithFeedback = new();
    
    private U__Unit currentUnit;
    
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
    
    /// <summary>
    /// Shows the tiles a character can attack.
    /// </summary>
    private void ShowFeedbacks(List<Tile> tilesToShow) => tilesToShow.
            ForEach(t => {
                t.SetMaterial(feedbackTileMaterial);
                tilesWithFeedback.Add(t);
            });
    
    /// <summary>
    /// Resets the tiles skin and clears the attackable tiles list.
    /// </summary>
    private void HideFeedbacks()
    {
        tilesWithFeedback.ForEach(t => t.ResetTileSkin());
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
        if (!startingUnit.actions.HasUsableAction<A_Attack>()) 
            return; // No attack action
        
        currentUnit = startingUnit;
        currentUnit.weaponHolder.OnWeaponChange += WeaponsHolder_OnWeaponChanged;
        ShowFeedbacks(startingUnit.attack.AttackableTiles());
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if (!endingUnit.behavior.playable) 
            return; // NPC
        if (!endingUnit.actions.HasUsableAction<A_Attack>()) 
            return; // No attack action
        
        currentUnit.weaponHolder.OnWeaponChange -= WeaponsHolder_OnWeaponChanged;
        HideFeedbacks();
        
        currentUnit = null;
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
        if (!endingActionUnit.actions.HasUsableAction<A_Attack>()) 
            return; // No attack action
        
        ShowFeedbacks(endingActionUnit.attack.AttackableTiles());
    }
    
    private void WeaponsHolder_OnWeaponChanged(object sender, Weapon newWeapon)
    {
        HideFeedbacks();
        ShowFeedbacks(_units.current.attack.AttackableTiles());
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        HideFeedbacks();
    }
}
