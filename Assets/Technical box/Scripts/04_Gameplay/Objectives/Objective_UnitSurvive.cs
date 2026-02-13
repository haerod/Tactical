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
public class Objective_UnitSurvive : Objective
{
    [Header("- VICTORY CONDITIONS -")][Space]

    [SerializeField] private Unit _unitToSurvive;

    private bool _isDead;
    
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
        if(_isDead)
            FailObjective();
    }
    
    protected override string GetDescription() => _description.Contains("[X]") ? 
        _description.Replace("[X]", _unitToSurvive.unitName) : 
        null;
    
    protected override void SubscribeToEvents()
    {
        GameEvents.OnAnyDeath += GameEvents_OnAnyDeath;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        GameEvents.OnAnyDeath -= GameEvents_OnAnyDeath;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyDeath(object sender, Unit deadUnit)
    {
        if (deadUnit == _unitToSurvive)
            _isDead = true;
        
        CheckObjectiveCompletion();
    }
}