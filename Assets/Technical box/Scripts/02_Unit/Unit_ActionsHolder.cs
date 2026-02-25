using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class Unit_ActionsHolder : MonoBehaviour
{
    [SerializeField] private int _actionPoints = 3;
    public int actionPoints => _actionPoints;
    
    [Header("REFERENCES")]
    [SerializeField] private Unit unit;

    public int currentActionPoints { get;  private set; }
    public List<A__Action> actions { get; private set; } = new();
    public bool areAvailableActions => AreAvailableActions();
    public List<A__Action> availableActions => GetAvailableActions();
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    private void Awake()
    {
        foreach (Transform child in transform)
            AddAction(child.GetComponent<A__Action>());
    }

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnStart += Units_OnTeamTurnStart;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
    }

    private void OnDisable()
    {
        actions.ToList().ForEach(RemoveAction);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

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
    public bool HasAvailableAction<T>() where T : A__Action => actions
            .Any(action => action is T && CanUse(action));

    /// <summary>
    /// Returns the Action of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetActionOfType<T>() where T : A__Action => actions
        .OfType<T>().FirstOrDefault();
    
    /// <summary>
    ///  Refills the current action points to maximum value.
    /// </summary>
    public void RefillActionPoints() => currentActionPoints = actionPoints;
    
    /// <summary>
    /// Adds the given amount to action points.
    /// </summary>
    /// <param name="_amount"></param>
    public void AddActionPoints(int _amount) => currentActionPoints += _amount;
    
    /// <summary>
    /// Removes the action points of the action.
    /// </summary>
    public void SpendActionPoints(A__Action action)
    {
        currentActionPoints -= action.actionPointCost;
        
        if (action.spendAllActionPoints)
            currentActionPoints = 0;
    }

    /// <summary>
    /// Returns true if the action can be spent and is available.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool CanUse(A__Action action) => CanSpend(action)
        && action.isAvailable
        && (action.actionPointCost == 0 && currentActionPoints == 0 ? action.usableWithoutActionPoints : true);

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
    /// Add the action to actions list.
    /// </summary>
    /// <param name="actionToAdd"></param>
    private void AddAction(A__Action actionToAdd)
    {
        actions.Add(actionToAdd);
        actionToAdd.OnActionStart += Action_OnActionStart;
        actionToAdd.OnActionEnd += Action_OnActionEnd;
    }

    /// <summary>
    /// Remove an action from action list.
    /// </summary>
    /// <param name="actionToRemove"></param>
    private void RemoveAction(A__Action actionToRemove)
    {
        actions.Remove(actionToRemove);
        actionToRemove.OnActionStart -= Action_OnActionStart;
        actionToRemove.OnActionEnd -= Action_OnActionEnd;
    }
    
    /// <summary>
    /// Returns true if an action can be used.
    /// </summary>
    /// <returns></returns>
    private bool AreAvailableActions() => actions.Any(CanUse);

    /// <summary>
    /// Returns unit's usable actions.
    /// </summary>
    /// <returns></returns>
    private List<A__Action> GetAvailableActions() => actions
        .Where(CanUse)
        .ToList();
    
    /// <summary>
    /// Returns true if it's enough action points to start action.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private bool CanSpend(A__Action action) => action.actionPointCost <= currentActionPoints;
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit )
    {
        if(startingUnit != unit)
            return; // Is not current unit
        
        if(unit.behavior.playable)
            SubscribeToEvents();
        else
            unit.behavior.PlayBehavior();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(endingUnit == unit)
            UnsubscribeToEvents();
    }
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if(unit.unitTeam != startingTeam)
            return; // Not the starting team
        
        RefillActionPoints();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        if(unit.unitTeam != endingTeam)
            return; // Not the starting team
        
        UnsubscribeToEvents();
    }
    
    private void Action_OnActionStart(object sender, A__Action startingAction)
    {
        SpendActionPoints(startingAction);
    }

    private void Action_OnActionEnd(object sender, A__Action endingAction)
    {
        
    }

}
