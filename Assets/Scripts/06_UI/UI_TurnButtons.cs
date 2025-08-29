using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class UI_TurnButtons : MonoBehaviour
{
    [SerializeField] private Button nextTurnButton;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        
        Turns.OnVictory += Turns_OnVictory;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Events call on Next Turn button's click
    /// </summary>
    public void ButtonNextTurn() => Turns.EndTurn();
    
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

    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if(!startingCharacter.behavior.playable)
            return; // NPC

        startingCharacter.move.OnMovementStart += Move_OnMovementStart;
        startingCharacter.move.OnMovementEnd += Move_OnMovementEnd;
        startingCharacter.attack.OnAttackStart += Attack_OnAttackStart;
        startingCharacter.attack.OnAttackEnd += Attack_OnAttackEnd;
        
        SetTurnButtonsActive(true);
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if(!endingCharacter.behavior.playable)
            return; // NPC
        
        endingCharacter.move.OnMovementStart -= Move_OnMovementStart;
        endingCharacter.move.OnMovementEnd -= Move_OnMovementEnd;
        endingCharacter.attack.OnAttackStart -= Attack_OnAttackStart;
        endingCharacter.attack.OnAttackEnd -= Attack_OnAttackEnd;
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Attack_OnAttackEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(_characters.current.behavior.playable);
    }
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }

    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
}
