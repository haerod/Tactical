using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class Module_TurnButtons : MonoBehaviour
{
    [SerializeField] private GameObject layoutGroup;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        SetUIActive(false);
    }
    
    private void Start()
    {
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        _level.OnVictory += Level_OnVictory;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Events call on End Turn button's click
    /// </summary>
    public void ButtonEndTurn() => _units.PassToNextTeam();

    /// <summary>
    /// Events call on Next Unit button's click
    /// </summary>
    public void ButtonNextUnit() => _units.PassToNextPlayableTeammate();
    
    /// <summary>
    /// Events call on Previous Unit button's click
    /// </summary>
    public void ButtonPreviousUnit() => _units.PassToPreviousPlayableTeammate();

    /// <summary>
    /// Events call on Rotate Camera Previous button's click
    /// </summary>
    public void ButtonRotateCameraPrevious() => _camera.RotateOnAngle(90f);
    
    /// <summary>
    /// Events call on Rotate Camera Next button's click
    /// </summary>
    public void ButtonRotateCameraNext() => _camera.RotateOnAngle(-90f);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables / disables player's UI.
    /// </summary>
    /// <param name="value"></param>
    private void SetUIActive(bool value) => layoutGroup.SetActive(value);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        SetUIActive(startingUnit.behavior.playable);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        SetUIActive(false);
    }
    
    private void Level_OnVictory(object sender, Team winnerTeam)
    {
        SetUIActive(false);
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        SetUIActive(false);
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit actingUnit)
    {
        SetUIActive(false);
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        SetUIActive(_units.current.behavior.playable);
    }
}
