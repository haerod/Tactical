using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class C_Actions : MonoBehaviour
{
    [SerializeField] private List<A__Action> actions;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Character's actions subscribes to Input's events.
    /// </summary>
    private void SubscribeToEvents() => actions.ForEach(action => action.SubscribeToEvents());

    /// <summary>
    /// Character's actions unsubscribes to Input's events.
    /// </summary>
    private void UnsubscribeToEvents() => actions.ForEach(action => action.UnsubscribeToEvents());

    /// <summary>
    /// Returns true if the character has Heal action.
    /// </summary>
    /// <returns></returns>
    public bool HasHealAction() => actions.OfType<A_Heal>().Any();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter )
    {
        if(startingCharacter != c)
            return; // Is not current character
        
        if(c.behavior.playable)
            SubscribeToEvents();
        else
            c.behavior.PlayBehavior();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if(endingCharacter == c)
            UnsubscribeToEvents();
    }
}
