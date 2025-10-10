using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class FM_TurnButtons : MonoBehaviour
{
    [SerializeField] private Button nextTurnButton;

    private U__Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
    }
    
    private void OnDisable()
    {
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        _rules.OnVictory -= Rules_OnVictory;
        
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
    /// Events call on Next Turn button's click
    /// </summary>
    public void ButtonNextTurn() => _units.EndCurrentUnitTurn();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables / disables player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    private void SetTurnButtonsActive(bool value) => nextTurnButton.gameObject.SetActive(value);
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        if(!startingCharacter.behavior.playable)
            return; // NPC

        currentUnit = startingCharacter;
        
        currentUnit.move.OnMovementStart += Move_OnMovementStart;
        currentUnit.move.OnMovementEnd += Move_OnMovementEnd;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
        currentUnit.attack.OnAttackEnd += Attack_OnAttackEnd;
        
        SetTurnButtonsActive(true);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingCharacter)
    {
        if(!endingCharacter.behavior.playable)
            return; // NPC
        
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.move.OnMovementEnd -= Move_OnMovementEnd;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        currentUnit.attack.OnAttackEnd -= Attack_OnAttackEnd;
        
        currentUnit = null;
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Attack_OnAttackEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(_units.current.behavior.playable);
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(_units.current.behavior.playable);
    }

    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
}
