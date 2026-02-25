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
public class Power_ActionOnKill : Power
{
    [Header("- FEEDBACKS -")][Space]
    
    [SerializeField] private GameObject _feedbackPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyDeath += GameEvents_OnAnyDeath;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private void WinActionPoint()
    {
        ExecutePower();
        _unit.actionsHolder.AddActionPoints(1);
        
        if(_feedbackPrefab)
            Instantiate(_feedbackPrefab, _unit.transform.position, Quaternion.identity);
    }
    
    protected override string GetDescription() => _description;
    protected override string GetTextFeedback() => _textFeedback;
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyDeath(object sender, Unit deadUnit)
    {
        if(_units.current != _unit)
            return; // Not its turn
        
        WinActionPoint();
    }
}