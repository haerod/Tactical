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
    [SerializeField] private U_Health health;
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InitialiseBar();

        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        
        health.OnDeath += Health_OnDeath;
        health.HealthChanged += Health_HealthChanged;
    }

    private void OnDisable()
    {
        health.OnDeath -= Health_OnDeath;
        health.HealthChanged -= Health_HealthChanged;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Displays the current life and max life on life bar.
    /// </summary>
    private void InitialiseBar()
    {
        maximumValue = health.health;
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
    
    /// <summary>
    /// Starts a waits for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits for "time" seconds and executes an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(startingUnit != unit)
            return; // Another character

        Show();
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(endingUnit != unit)
            return; // Another character
        
        Hide();
    }
    
    private void InputEvents_OnUnitEnter(object sender, U__Unit hoveredUnit)
    {
        if(hoveredUnit != unit)
            return; // Another character
        
        U__Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit == unit)
            return; // Current unit
        if(!currentUnit.look.UnitsVisibleInFog().Contains(unit))
            return; // Invisible unit
        
        Show();
    }
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        Wait(1, Hide);
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
        U__Unit currentUnit = _units.current;
        
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