using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;

public class M_Units : MonoBehaviour
{
    [HideInInspector] public U__Unit current;
    [SerializeField] private List<Team> teamPlayOrder;
    [SerializeField] private List<U__Unit> characters;
    [SerializeField] private TurnBasedSystem turnBasedSystem;
    
    public event EventHandler<U__Unit> OnUnitTurnStart;
    public event EventHandler<U__Unit> OnUnitTurnEnd;
    
    public static M_Units instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Characters in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }
    
    private void Start()
    {
        _input.OnChangeCharacterInput += Input_OnChangeCharacterInput;
        _input.OnEndTurnInput += Input_OnEndTurnInput;
        StartUnitTurn(turnBasedSystem.GetFirstCharacter());
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the full units list.
    /// </summary>
    /// <returns></returns>
    public List<U__Unit> GetUnitsList() => characters;
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<U__Unit> GetAlliesOf(U__Unit unit) => characters
        .Where(testedUnit => testedUnit.team.IsAllyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<U__Unit> GetEnemiesOf(U__Unit unit) => characters
        .Where(testedUnit => testedUnit.team.IsEnemyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all the units of the given team.
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<U__Unit> GetUnitsOf(Team team) => GetUnitsList()
        .Where(testedUnit => testedUnit.unitTeam == team)
        .ToList();
    
    /// <summary>
    /// Returns the teams play order.
    /// </summary>
    /// <returns></returns>
    public List<Team> GetTeamPlayOrder() => teamPlayOrder;
    
    /// <summary>
    /// Adds a new unit in the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(U__Unit unit)
    {
        GetUnitsList().Add(unit);
        
        if(!teamPlayOrder.Contains(unit.unitTeam))
            teamPlayOrder.Add(unit.unitTeam);
    }
    
    /// <summary>
    /// Removes a unit from the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(U__Unit unit)
    {
        GetUnitsList().Remove(unit);
        
        if(IsAnotherUnitOfTheSameTeam(unit))
            return; // Is another unit of the same team
        
        teamPlayOrder.Remove(unit.unitTeam);
    }
    
    /// <summary>
    /// Ends the current unit's turn and passes to the next one (depending on the Turn Based System).
    /// If it's an overrideNextUnit, passes to this unit.
    /// <param name="overrideNextUnit"></param>
    /// </summary>
    public void EndCurrentUnitTurn(U__Unit overrideNextUnit = null)
    {
        if (_rules.IsVictory()) // Victory
            return;

        OnUnitTurnEnd?.Invoke(this, current);
        
        U__Unit nextUnit = overrideNextUnit ? overrideNextUnit : turnBasedSystem.GetNextUnit();

        if (!nextUnit.team.IsTeammateOf(current))
        {
            EndCurrentTeamTurn();
            StartNextTeamTurn(nextUnit.unitTeam);
        }
        
        StartUnitTurn(nextUnit);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Starts the turn of a given unit.
    /// <param name="newCurrentUnit"></param>
    /// </summary>
    private void StartUnitTurn(U__Unit newCurrentUnit)
    {
        current = newCurrentUnit;
        
        OnUnitTurnStart?.Invoke(this, newCurrentUnit);
    }

    /// <summary>
    /// Returns true if is another unit in the given unit team. Else returns false.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool IsAnotherUnitOfTheSameTeam(U__Unit unit) =>GetUnitsList()
            .Where(testedUnit => testedUnit != unit)
            .FirstOrDefault(testedUnit => testedUnit.team.IsAllyOf(unit));

    private void StartNextTeamTurn(Team nextTeam)
    {
        GetUnitsOf(nextTeam)
            .ForEach(unit => unit.SetCanPlayValue(true));
    }
    
    private void EndCurrentTeamTurn()
    {
        GetUnitsOf(current.unitTeam)
            .ForEach(unit => unit.SetCanPlayValue(true));
    }
    
    private void SwitchToNextTeamUnit()
    {
        U__Unit nextTeamUnit = turnBasedSystem.NextTeamUnit();
        
        if(!nextTeamUnit)
            return; // No other team unit
        
        EndCurrentUnitTurn(nextTeamUnit);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Input_OnEndTurnInput(object sender, EventArgs e)
    {
        EndCurrentTeamTurn();
        EndCurrentUnitTurn();
    }
    
    private void Input_OnChangeCharacterInput(object sender, EventArgs e)
    {
        SwitchToNextTeamUnit();
    }
}
