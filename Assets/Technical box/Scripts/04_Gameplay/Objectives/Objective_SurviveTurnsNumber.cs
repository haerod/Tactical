using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Serialization;
using static Utils;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Objective_SurviveTurnsNumber : Objective
{
    [Header("- VICTORY CONDITIONS -")][Space]
    
    [SerializeField] private int _turnsToSurvive = 3;
    [SerializeField] private Team _survivingTeam;
    
    private int _currentTurn;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    protected override void CheckObjectiveCompletion()
    {
        InvokeOnObjectiveUpdate(this);
        
        if(_units.GetUnitsOf(_survivingTeam).Count == 0)
        {
            FailObjective();
            return; // Every survivor is dead
        }
        
        if(_level.currentTurn >= _turnsToSurvive)
        {
            SuccessObjective();
            return; // Not the good turn
        }
    }
    
    protected override string GetDescription() => _description.Contains("[X]") ? 
        _description.Replace("[X]", (_turnsToSurvive - _currentTurn +1).ToString()) : 
        null;
    
    protected override void SubscribeToEvents()
    {
        _level.OnNewTurnStart += Level_OnNewTurnStart;
        GameEvents.OnAnyDeath += GameEvents_OnAnyDeath;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        _level.OnNewTurnStart -= Level_OnNewTurnStart;
        GameEvents.OnAnyDeath -= GameEvents_OnAnyDeath;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Level_OnNewTurnStart(object sender, EventArgs e)
    {
        _currentTurn = _level.currentTurn;
        CheckObjectiveCompletion();
    }
    
    private void GameEvents_OnAnyDeath(object sender, Unit deadUnit)
    {
        CheckObjectiveCompletion();
    }
}