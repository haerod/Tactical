using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class Module_TurnButtons : MonoBehaviour
{
    [SerializeField] private GameObject layoutGroup;

    private U__Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
    }

    private void OnDisable()
    {
        if(!currentUnit)
            return;
        
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.move.OnMovementEnd -= Move_OnMovementEnd;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        currentUnit.attack.OnAttackEnd -= Attack_OnAttackEnd;
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

    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC

        currentUnit = startingUnit;
        
        currentUnit.move.OnMovementStart += Move_OnMovementStart;
        currentUnit.move.OnMovementEnd += Move_OnMovementEnd;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
        currentUnit.attack.OnAttackEnd += Attack_OnAttackEnd;
        
        SetUIActive(true);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        SetUIActive(false);
        
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.move.OnMovementEnd -= Move_OnMovementEnd;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        currentUnit.attack.OnAttackEnd -= Attack_OnAttackEnd;
        
        currentUnit = null;
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        SetUIActive(false);
    }
    
    private void Attack_OnAttackEnd(object sender, EventArgs e)
    {
        SetUIActive(_units.current.behavior.playable);
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        SetUIActive(false);
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        SetUIActive(_units.current.behavior.playable);
    }

    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        SetUIActive(false);
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        SetUIActive(false);
    }
}
