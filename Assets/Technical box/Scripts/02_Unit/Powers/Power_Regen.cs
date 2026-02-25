using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Power_Regen : Power
{
    [Header("- FEEDBACKS -")]
    
    [SerializeField] private GameObject _feedbackPrefab;
        
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        //_units.OnTeamTurnStart += Units_OnTeamTurnStart;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Regenerates to full health.
    /// </summary>
    private void Regen()
    {
        Unit_Health _health = _unit.health;
        
        ExecutePower();
        _health.Heal(_health.missingHealth);
        Instantiate(_feedbackPrefab, _unit.transform.position, Quaternion.identity);
    }
    
    protected override string GetDescription() => _description;
    
    protected override string GetTextFeedback() => _unit.health.missingHealth > 0 ? 
        ColoredText($"+{_unit.health.missingHealth}HP healed", _textFeedbackColor) : 
        ColoredText($"Full HP", _textFeedbackColor);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if (startingUnit != _unit)
            return; // Not this unit
        if (_unit.health.isDead)
        {
            _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
            return; // Unit is dead
        }
        
        Regen();
    }
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if (startingTeam != _unit.unitTeam)
            return; // Not this team
        if (_unit.health.isDead)
        {
            _units.OnTeamTurnStart -= Units_OnTeamTurnStart;
            return; // Unit is dead
        }
        
        Regen();
    }
}