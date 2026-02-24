using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Module_SliceHealthBar : UI_SegmentedGaugeClamped
{
    [SerializeField] private int _minHealthValue = 2;
    [Range(0,1)][SerializeField] private float _minSizePercent = .5f;
    [SerializeField] private int _maxHealthValue = 5;
    [Range(0,1)][SerializeField] private float _maxSizePercent = 1f;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Unit _unit;
    [SerializeField] private RectTransform _parentRectTransform;
    private Unit_Health _health;
    
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
        
        _health.OnHealthChanged += Health_HealthChanged;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current life and max life on life bar.
    /// </summary>
    private void InitialiseBar()
    {
        _health = _unit.health;
        
        maximumValue = _health.healthMax;
        FillGauge(_health.currentHealth);
        
        float _sizePercent = Mathf.Lerp(
            _minSizePercent,
            _maxSizePercent,
            Mathf.Clamp01(PercentInRange(_health.healthMax, _minHealthValue, _maxHealthValue)));
        ScaleStretchedUIInPercent(_parentRectTransform, _sizePercent, 1);
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
        if(startingUnit != _unit)
            return; // Another character

        Show();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(endingUnit != _unit)
            return; // Another character
        
        Hide();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredUnit)
    {
        if(hoveredUnit != _unit)
            return; // Another character
        
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit == _unit)
            return; // Current unit
        if(!currentUnit.look.UnitsVisibleInFog().Contains(_unit))
            return; // Invisible unit
        
        Show();
    }
    
    private void Health_HealthChanged(object sender, EventArgs e)
    {
        FillGauge(_health.currentHealth);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        if(exitedTile.character != _unit)
            return; // Not this character
        if(_units.current == _unit)
            return; // Is the current character
        
        Hide();
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit.team.IsTeammateOf(_unit))
            return; // Teammate
        if(!currentUnit.look.CanSee(_unit))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(_unit.look.visibleTiles.Contains(enteredTile))
            Hide();
        else
            Show();
    }
}