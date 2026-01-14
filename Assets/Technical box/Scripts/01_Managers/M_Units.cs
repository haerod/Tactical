using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Units : MonoBehaviour
{
    [HideInInspector] public U__Unit current;
    [SerializeField] private List<Team> teamPlayOrder;
    [SerializeField] private List<U__Unit> units;
    [SerializeField] private TurnBasedSystem turnBasedSystem;
    
    public event EventHandler<U__Unit> OnUnitTurnStart;
    public event EventHandler<U__Unit> OnUnitTurnEnd;
    public event EventHandler<Team> OnTeamTurnStart;
    public event EventHandler<Team> OnTeamTurnEnd;
    
    public static M_Units instance => _instance == null ? FindFirstObjectByType<M_Units>() : _instance;
    public static M_Units _instance;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!_instance)
            _instance = this;
        else
            Debug.LogError("There is more than one M_Characters in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        
        units = transform.Cast<Transform>().Select(t => t.GetComponent<U__Unit>()).ToList();
    }
    
    private void Start()
    {
        StartCoroutine(LateStart_Coroutine());
    }
    
    private void LateStart()
    {
        _input.OnNextTeammateInput += Input_OnNextTeammateInput;
        _input.OnEndTeamTurnInput += Input_OnEndTeamTurnInput;

        U__Unit firstUnit = turnBasedSystem.GetFirstUnit();
        StartTeamTurn(firstUnit.unitTeam);
        StartUnitTurn(firstUnit);
    }

    private void OnDisable()
    {
        OnUnitTurnStart = null;
        OnUnitTurnEnd = null;
        OnTeamTurnStart = null;
        OnTeamTurnEnd = null;
        
        GameEvents.ClearAllEvents();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the full units list.
    /// </summary>
    /// <returns></returns>
    public List<U__Unit> GetUnitsList() => units;
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<U__Unit> GetAlliesOf(U__Unit unit) => units
        .Where(testedUnit => testedUnit.team.IsAllyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<U__Unit> GetEnemiesOf(U__Unit unit) => units
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

        if (overrideNextUnit)
        {
            StartUnitTurn(overrideNextUnit);
            return;
        }
        
        U__Unit nextUnit = turnBasedSystem.GetNextPlayableTeammate();

        if (nextUnit == current)
            PassToNextTeam();
        else
            StartUnitTurn(nextUnit);
    }

    /// <summary>
    /// Ends the turn of the current unit's team, and start the next one.
    /// </summary>
    public void PassToNextTeam()
    {
        Team nextTeam = turnBasedSystem.GetNextTeam();
        EndCurrentTeamTurn();
        StartTeamTurn(nextTeam);
        StartUnitTurn(GetUnitsOf(nextTeam).First());
    }
    
    /// <summary>
    /// Ends current unit's turn and pass to the next playable one in its team. 
    /// </summary>
    public void PassToNextPlayableTeammate()
    {
        U__Unit nextUnit = turnBasedSystem.GetNextPlayableTeammate();
   
        if(nextUnit)
            EndCurrentUnitTurn(nextUnit);
    }
    
    /// <summary>
    /// Ends current unit's turn and pass to the previous playable one in its team. 
    /// </summary>
    public void PassToPreviousPlayableTeammate()
    {
        U__Unit nextUnit = turnBasedSystem.GetPreviousPlayableTeammate();
   
        if(nextUnit)
            EndCurrentUnitTurn(nextUnit);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private IEnumerator LateStart_Coroutine()
    {
        yield return new WaitForEndOfFrame();
        LateStart();
    }
    
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
    
    /// <summary>
    /// Starts the given team's turn.
    /// </summary>
    /// <param name="team"></param>
    private void StartTeamTurn(Team team)
    {
        OnTeamTurnStart?.Invoke(this, team);
        
        GetUnitsOf(team)
            .ForEach(unit => unit.SetCanPlayValue(true));
    }
    
    /// <summary>
    /// Ends the current team's turn.
    /// </summary>
    private void EndCurrentTeamTurn()
    {
        OnTeamTurnEnd?.Invoke(this, current.unitTeam);
        
        GetUnitsOf(current.unitTeam)
            .ForEach(unit => unit.SetCanPlayValue(false));
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Input_OnEndTeamTurnInput(object sender, EventArgs e)
    {
        PassToNextTeam();
    }
    
    private void Input_OnNextTeammateInput(object sender, EventArgs e)
    {
        PassToNextPlayableTeammate();
    }
}
