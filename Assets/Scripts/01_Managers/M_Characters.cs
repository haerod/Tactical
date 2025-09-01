using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;

public class M_Characters : MonoBehaviour
{
    [HideInInspector] public C__Character current;
    [SerializeField] private List<Team> teamPlayOrder;
    [SerializeField] private List<C__Character> characters;
    [SerializeField] private TurnBasedSystem turnBasedSystem;
    
    public event EventHandler<C__Character> OnCharacterTurnStart;
    public event EventHandler<C__Character> OnCharacterTurnEnd;
    
    public static M_Characters instance;

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
    public List<C__Character> GetUnitsList() => characters;
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<C__Character> GetAlliesOf(C__Character unit) => characters
        .Where(testedUnit => testedUnit.team.IsAllyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<C__Character> GetEnemiesOf(C__Character unit) => characters
        .Where(testedUnit => testedUnit.team.IsEnemyOf(unit))
        .ToList();
    
    public List<C__Character> GetUnitsOf(Team team) => GetUnitsList()
        .Where(testedUnit => testedUnit.unitTeam == team)
        .ToList();
    
    /// <summary>
    /// Returns the teams play order.
    /// </summary>
    /// <returns></returns>
    public List<Team> GetTeamsPlayOrder() => teamPlayOrder;
    
    /// <summary>
    /// Adds a new unit in the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(C__Character unit)
    {
        GetUnitsList().Add(unit);
        
        if(!teamPlayOrder.Contains(unit.unitTeam))
            teamPlayOrder.Add(unit.unitTeam);
    }
    
    /// <summary>
    /// Removes a unit from the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(C__Character unit)
    {
        GetUnitsList().Remove(unit);
        
        if(IsAnotherUnitOfTheSameTeam(unit))
            return; // Is another unit of the same team
        
        teamPlayOrder.Remove(unit.unitTeam);
    }
    
    /// <summary>
    /// Starts the turn of a given unit.
    /// </summary>
    public void StartUnitTurn(C__Character newCurrentUnit)
    {
        current = newCurrentUnit;
        
        OnCharacterTurnStart?.Invoke(this, newCurrentUnit);
    }
    
    /// <summary>
    /// Ends the current unit's turn and passes to the next one (depending on the Turn Based System).
    /// </summary>
    public void EndCurrentUnitTurn()
    {
        if (_rules.IsVictory()) // Victory
            return;

        OnCharacterTurnEnd?.Invoke(this, current);
        
        C__Character nextUnit = turnBasedSystem.GetNextUnit();

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
    /// Returns true if is another unit in the given unit team. Else returns false.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool IsAnotherUnitOfTheSameTeam(C__Character unit) =>GetUnitsList()
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
        C__Character nextTeamUnit = turnBasedSystem.NextTeamUnit();
        
        if(!nextTeamUnit)
            return; // No other team unit
        
        EndCurrentUnitTurn();
        StartUnitTurn(nextTeamUnit);
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
