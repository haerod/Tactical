using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class U_Actions : MonoBehaviour
{
    [SerializeField] private List<A__Action> actions;
    
    [Header("REFERENCES")]
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Unit's actions subscribes to Input's events.
    /// </summary>
    private void SubscribeToEvents() => actions.ForEach(action => action.SubscribeToEvents());

    /// <summary>
    /// Unit's actions unsubscribes to Input's events.
    /// </summary>
    private void UnsubscribeToEvents() => actions.ForEach(action => action.UnsubscribeToEvents());

    /// <summary>
    /// Returns true if the unit has Heal action.
    /// </summary>
    /// <returns></returns>
    public bool HasHealAction() => actions.OfType<A_Heal>().Any();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit )
    {
        if(startingUnit != unit)
            return; // Is not current unit
        
        if(unit.behavior.playable)
            SubscribeToEvents();
        else
            unit.behavior.PlayBehavior();
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(endingUnit == unit)
            UnsubscribeToEvents();
    }
}
