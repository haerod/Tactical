using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_ObjectivesDisplayer : MonoBehaviour
{
    [SerializeField] private Transform _objectiveParent;
    [SerializeField] private GameObject _objectivePrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        _units.OnTeamTurnStart += Units_OnTeamTurnStart;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        
        Show();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the objectives.
    /// </summary>
    private void Show()
    {
        Hide();
        
        Module_Objective instantiatedObjective = Instantiate(_objectivePrefab, _objectiveParent)
            .GetComponent<Module_Objective>();
        
        instantiatedObjective.Setup();
    }
    
    /// <summary>
    /// Destroys all objectives in children.
    /// </summary>
    private void Hide()
    {
        foreach (Transform child in _objectiveParent)
            Destroy(child.gameObject);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyActionStart(object sender, Unit startingUnit)
    {
        Hide();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingUnit)
    {
        if(!_units.current.behavior.playable)
            return; // NPC
        
        Show();
    }
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if(!_units.current)
            return; // No current unit
        if(!_units.current.behavior.playable)
            return; // NPC
        
        Show();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
}