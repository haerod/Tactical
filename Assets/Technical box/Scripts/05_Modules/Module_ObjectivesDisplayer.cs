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
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Transform _objectiveParent;
    [SerializeField] private GameObject _objectivePrefab;
    [SerializeField] private GameObject _objectiveTextPrefab;
    [SerializeField] private GameObject _optionalObjectiveTextPrefab;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        _level.OnObjectiveUpdate += Level_OnObjectiveUpdate;
        _level.OnDefeat += Level_OnDefeat;
        
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

        // Main objectives
        if (_level.objectives.Any(objective => !objective.isOptional))
        {
            Instantiate(_objectiveTextPrefab, _objectiveParent);
        
            foreach (Objective objective in _level.objectives)
            {
                if(objective.isOptional)
                    continue; // Optional objective
            
                DisplayObjective(objective);
            }
        }
        
        // Optional objectives
        if(!_level.objectives.Any(objective => objective.isOptional))
            return; // No optional objective
        
        Instantiate(_optionalObjectiveTextPrefab, _objectiveParent);
        
        foreach (Objective objective in _level.objectives)
        {
            if(!objective.isOptional)
                continue; // Main objective

            DisplayObjective(objective);
        }

        void DisplayObjective(Objective objective)
        {
            Module_Objective instantiatedObjective = Instantiate(_objectivePrefab, _objectiveParent)
                .GetComponent<Module_Objective>();

            instantiatedObjective.Display(
                objective.description, 
                (objective.successOnVictory && _level.isVictory) || objective.isCompleted,
                objective.isSuccessful);
        }
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
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        Show();
    }
    
    private void Level_OnObjectiveUpdate(object sender, EventArgs e)
    {
        if(!_units.current)
            return; // No current unit
        if(!_units.current.behavior.playable)
            return; // NPC
        
        Show();
    }
    
    private void Level_OnDefeat(object sender, EventArgs e)
    {
        Show();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if(!_units.current.behavior.playable)
            return; // NPC
        
        Show();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        Hide();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
}