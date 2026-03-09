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
public class Objective_TeamDontKillTeam : Objective
{
    [SerializeField] private Team _teamAttacker;
    [SerializeField] private Team _teamToNoKill;
    
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
        if(deadUnit.unitTeam != _teamToNoKill)
            return; // Unit can be killed
        if(_units.current.unitTeam != _teamAttacker)
            return; // Not attacker team
        
        CheckObjectiveCompletion();
    }
}