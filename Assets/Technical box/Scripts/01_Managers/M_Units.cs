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
    [HideInInspector] public Unit current;
    [SerializeField] private List<Team> _teamPlayOrder;
    public List<Team> teamPlayOrder => _teamPlayOrder;
    
    [Space]
    
    [SerializeField] private TurnBasedSystem turnBasedSystem;

    [Space]
    
    [SerializeField] private float _timeBeforeStart = .5f;
    
    [Header("- DEBUG -")] [Space]
    
    [SerializeField] private List<Unit> _units;
    public List<Unit> units => _units;
    
    public event EventHandler<Unit> OnUnitTurnStart;
    public event EventHandler<Unit> OnUnitTurnEnd;
    public event EventHandler<Team> OnTeamTurnStart;
    public event EventHandler<Team> OnTeamTurnEnd;
    
    private static M_Units _instance;
    public static M_Units instance => _instance == null ? FindFirstObjectByType<M_Units>() : _instance;
    
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
        
        _units = transform.Cast<Transform>().Select(t => t.GetComponent<Unit>()).ToList();
    }
    
    private void Start()
    {
        _input.OnNextTeammateInput += Input_OnNextTeammateInput;
        _input.OnEndTeamTurnInput += Input_OnEndTeamTurnInput;

        _timeBeforeStart = Mathf.Clamp(_timeBeforeStart, .1f, Mathf.Infinity);
        Wait(_timeBeforeStart, StartGame);
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
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<Unit> GetAlliesOf(Unit unit) => _units
        .Where(testedUnit => testedUnit.team.IsAllyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<Unit> GetEnemiesOf(Unit unit) => _units
        .Where(testedUnit => testedUnit.team.IsEnemyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all the units of the given team.
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public List<Unit> GetUnitsOf(Team team) => units
        .Where(testedUnit => testedUnit.unitTeam == team)
        .ToList();
    
    /// <summary>
    /// Adds a new unit in the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.transform.SetParent(transform);
        _units = transform.Cast<Transform>().Select(t => t.GetComponent<Unit>()).ToList();

        if(!_teamPlayOrder.Contains(unit.unitTeam))
            _teamPlayOrder.Add(unit.unitTeam);
    }
    
    /// <summary>
    /// Removes a unit from the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
        unit.transform.SetParent(null);
        _units = transform.Cast<Transform>().Select(t => t.GetComponent<Unit>()).ToList();
        
        // if(IsAnotherUnitOfTheSameTeam(unit))
        //     return; // Is another unit of the same team
        //
        // teamPlayOrder.Remove(unit.unitTeam);
    }
    
    /// <summary>
    /// Ends the current unit's turn and passes to the next one (depending on the Turn Based System).
    /// If it's an overrideNextUnit, passes to this unit.
    /// <param name="overrideNextUnit"></param>
    /// </summary>
    public void EndCurrentUnitTurn(Unit overrideNextUnit = null)
    {
        if (_level.isVictory) // Victory
            return;

        OnUnitTurnEnd?.Invoke(this, current);

        if (overrideNextUnit)
        {
            StartUnitTurn(overrideNextUnit);
            return;
        }
        
        Unit nextUnit = turnBasedSystem.GetNextPlayableTeammate();

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
        Unit nextUnit = turnBasedSystem.GetNextPlayableTeammate();
   
        if(nextUnit)
            EndCurrentUnitTurn(nextUnit);
    }
    
    /// <summary>
    /// Ends current unit's turn and pass to the previous playable one in its team. 
    /// </summary>
    public void PassToPreviousPlayableTeammate()
    {
        Unit nextUnit = turnBasedSystem.GetPreviousPlayableTeammate();
   
        if(nextUnit)
            EndCurrentUnitTurn(nextUnit);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Starts the turn of the first unit.
    /// </summary>
    private void StartGame()
    {
        Unit firstUnit = turnBasedSystem.GetFirstUnit();
        StartTeamTurn(firstUnit.unitTeam);
        StartUnitTurn(firstUnit);
    }
    
    /// <summary>
    /// Starts the turn of a given unit.
    /// <param name="newCurrentUnit"></param>
    /// </summary>
    private void StartUnitTurn(Unit newCurrentUnit)
    {
        current = newCurrentUnit;
        
        OnUnitTurnStart?.Invoke(this, newCurrentUnit);
    }

    /// <summary>
    /// Returns true if is another unit in the given unit team. Else returns false.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool IsAnotherUnitOfTheSameTeam(Unit unit) =>units
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
    
    /// <summary>
    /// Starts a wait for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    protected void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits coroutine.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
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
