using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;
using Object = UnityEngine.Object;

/// <summary>
/// Class description
/// </summary>
public class Objective_UnitGroupSurvive : Objective
{
    [Header("- VICTORY CONDITIONS -")][Space]
    
    [SerializeField] private List<Unit> _unitsToSurvive;
    [SerializeField] private bool _allMustSurvive;
    
    private bool _isDead;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    protected override void CheckObjectiveCompletion()
    {
        if(_isDead)
            FailObjective();
    }
    
    protected override string GetDescription() => _description;
    
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
        if (_unitsToSurvive.Contains(deadUnit))
        {
            if(_allMustSurvive || _unitsToSurvive.All(unit => unit.health.isDead))
                _isDead = true;
        }
        
        CheckObjectiveCompletion();
    }
}