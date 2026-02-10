using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_SliceHealthBar : UI_SegmentedGaugeClamped
{
    [SerializeField] private Unit_Health health;
    [SerializeField] private Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InitialiseBar();

        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        
        health.OnHealthChanged += Health_HealthChanged;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current life and max life on life bar.
    /// </summary>
    private void InitialiseBar()
    {
        maximumValue = health.healthMax;
        FillGauge(health.currentHealth);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables the life bar.
    /// </summary>
    private void Show () => itemParent.gameObject.SetActive(true);
    
    /// <summary>
    /// Disables the life bar.
    /// </summary>
    private void Hide() => itemParent.gameObject.SetActive(false);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(startingUnit != unit)
            return; // Another character

        Show();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(endingUnit != unit)
            return; // Another character
        
        Hide();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredUnit)
    {
        if(hoveredUnit != unit)
            return; // Another character
        
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit == unit)
            return; // Current unit
        if(!currentUnit.look.UnitsVisibleInFog().Contains(unit))
            return; // Invisible unit
        
        Show();
    }
    
    private void Health_HealthChanged(object sender, EventArgs e)
    {
        FillGauge(health.currentHealth);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        if(exitedTile.character != unit)
            return; // Not this character
        if(_units.current == unit)
            return; // Is the current character
        
        Hide();
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit.team.IsTeammateOf(unit))
            return; // Teammate
        if(!currentUnit.look.CanSee(unit))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(unit.look.visibleTiles.Contains(enteredTile))
            Hide();
        else
            Show();
    }
}