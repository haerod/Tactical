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
        A_Attack.OnAnyAttackStart += Attack_OnAnyAttackStart;
        A_Attack.OnAnyAttackEnd += Attack_OnAnyAttackEnd;
        Turns.OnVictory += Turns_OnVictory;
        A_Move.OnAnyMovementStart += Move_OnAnyMovementStart;
        A_Move.OnAnyMovementEnd += Move_OnAnyMovementEnd;
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
        SetTurnButtonsActive(startingCharacter.behavior.playable);
    }
    
    private void Attack_OnAnyAttackStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Attack_OnAnyAttackEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(_characters.current.behavior.playable);
    }
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }
    
    private void Move_OnAnyMovementEnd(object sender, EventArgs e)
    {
        SetTurnButtonsActive(false);
    }

    private void Move_OnAnyMovementStart(object sender, EventArgs e)
    {
        SetTurnButtonsActive(_characters.current.behavior.playable);
    }
}
