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
        _units.OnTeamTurnStart += Units_OnTeamTurnStart;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Sets the given actions usable.
    /// </summary>
    /// <param name="usableActions"></param>
    public void SetActionsUsabilityOf(List<A__Action> usableActions) => actions
        .ForEach(action => action.SetCanUseAction(usableActions.Contains(action)));
    
    /// <summary>
    /// Returns true if the list contains an Action of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasAction<T>() where T : A__Action => actions
        .Any(action => action is T);
    
    /// <summary>
    /// Returns true if the list contains an Action of the given type, and this one is usable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasUsableAction<T>() where T : A__Action => actions
        .Any(action => action is T && action.CanUse());
    
    /// <summary>
    /// Returns the Action of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetActionOfType<T>() where T : A__Action => actions
        .OfType<T>().FirstOrDefault();
    
    // ======================================================================
    // PRIVATE METHODS
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
    /// Sets starting actions usable.
    /// </summary>
    private void EnableStartingActions() => SetActionsUsabilityOf(actions
            .Where(action => action.isUsableOnStart)
            .ToList());
    
    /// <summary>
    /// Disables all the unit's action.
    /// </summary>
    private void DisableAllActions() => SetActionsUsabilityOf(new List<A__Action>());
    
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
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if(unit.unitTeam != startingTeam)
            return; // Not the starting team
        
        EnableStartingActions();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        if(unit.unitTeam != endingTeam)
            return; // Not the starting team
        
        DisableAllActions();    
    }
}
