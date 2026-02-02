using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_Objective : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private GameObject _checkMark;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void OnDisable()
    {
        _level.OnVictory -= Level_OnVictory;
        _level.OnNewTurnStart -= Level_OnNewTurnStart;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Setups the text depending on the objective.
    /// </summary>
    public void Setup()
    {
        _level.OnVictory += Level_OnVictory;
        _level.OnNewTurnStart += Level_OnNewTurnStart;
        
        if(_level.victoryCondition == M_Level.VictoryCondition.Deathmatch)
            _text.text = $"Kill all enemies";
        if(_level.victoryCondition == M_Level.VictoryCondition.ReachZone)
            _text.text = $"Reach the yellow zone";
        if (_level.victoryCondition == M_Level.VictoryCondition.Survive)
            _text.text = $"Survive {_level.turnsToSurvive - _level.currentTurn} turns";
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds a checkmark on the objective.
    /// </summary>
    private void CheckObjective()
    {
        _checkMark.SetActive(true);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Level_OnVictory(object sender, Team winningTeam)
    {
        CheckObjective();
    }
    
    private void Level_OnNewTurnStart(object sender, EventArgs e)
    {
        if (_level.victoryCondition == M_Level.VictoryCondition.Survive)
            _text.text = $"Survive {_level.turnsToSurvive - _level.currentTurn} turns";
    }
}