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
public class Objective_Deathmatch : Objective
{
    [Header("- VICTORY CONDITIONS -")][Space]
    
    [SerializeField] private Team _teamHasToWin;
    
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
        if (_units.GetUnitsOf(_teamHasToWin).Count == 0)
        {
            FailObjective();
            return; // Every team member is dead
        }

        if (_units.GetEnemiesOf(_units.GetUnitsOf(_teamHasToWin).First()).Count == 0)
        {
            SuccessObjective();
            return; // All enemies are dead
        }
    }
    
    protected override string GetDescription() => null;
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
    
    private void GameEvents_OnAnyDeath(object sender, Unit e)
    {
        CheckObjectiveCompletion();
    }
}