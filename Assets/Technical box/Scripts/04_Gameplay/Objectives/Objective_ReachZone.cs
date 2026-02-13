using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Objective_ReachZone : Objective
{
    [Header("- VICTORY CONDITIONS -")][Space]
    
    [SerializeField] private List<Tile> _tilesToReach;
    [SerializeField] private Unit _unitReaching;
    
    public List<Tile> tilesToReach => _tilesToReach;
    
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
        if (_unitReaching.health.isDead)
        {
            FailObjective();
            return; // Unit is dead
        }
        
        if(!_tilesToReach.Contains(_unitReaching.tile))
            return; // Not on a tile to reach.
        
        isCompleted = true;
        isSuccessful = true;
        InvokeOnObjectiveUpdate(this);
    }
    
    protected override string GetDescription() => _description.Contains("[X]") ? 
        _description.Replace("[X]", _unitReaching.unitName) : 
        null;
    
    protected override void SubscribeToEvents()
    {
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        GameEvents.OnAnyDeath += GameEvents_OnAnyDeath;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        GameEvents.OnAnyDeath -= GameEvents_OnAnyDeath;
        GameEvents.OnAnyActionEnd -= GameEvents_OnAnyActionEnd;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingUnit)
    {
        CheckObjectiveCompletion();
    }
    
    private void GameEvents_OnAnyDeath(object sender, Unit deadUnit)
    {
        CheckObjectiveCompletion();
    }
}