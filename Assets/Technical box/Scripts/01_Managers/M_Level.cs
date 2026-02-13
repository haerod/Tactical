using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static M__Managers;
using Object = System.Object;

public class M_Level : MonoBehaviour
{
    [Header("- FOG OF WAR -")] [Space]
    
    [SerializeField] private FogOfWar _fogOfWar;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    private static M_Level _instance;
    public static M_Level instance => _instance == null ? _instance = FindFirstObjectByType<M_Level>() : _instance;
    
    public bool isFogOfWar => _fogOfWar && _fogOfWar.gameObject.activeInHierarchy;
    public bool isVictory {get; private set;}
    public int currentTurn {get; private set;}
    public List<Objective> objectives => _objectives.Count > 0 ? _objectives : _objectives = GetObjectives();
    private List<Objective> _objectives = new();
    
    public event EventHandler OnVictory;
    public event EventHandler OnDefeat;
    public event EventHandler OnObjectiveUpdate;
    public event EventHandler OnNewTurnStart;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!_instance)
            _instance = this;
        else
            Debug.LogError("There is more than one M_Rules in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        
        _fogOfWar.gameObject.SetActive(true);
    }
    
    private void Start()
    {
        _units.OnTeamTurnStart += Units_OnTeamTurnStart;
        
        foreach (Objective objective in _objectives)
        {
            objective.OnObjectiveUpdate += Objective_OnObjectiveUpdate;
        }
    }
    
    private void OnDisable()
    {
        OnVictory = null;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the Objective of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetObjectiveOfType<T>() where T : Objective => objectives
        .OfType<T>().FirstOrDefault();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    private void IsVictoryOrDefeat()
    {
        if(objectives.Count == 0)
            return; // No objectives
        
        IEnumerable<Objective> mainObjectives = objectives
            .Where(objective => !objective.isOptional);
        
        if (mainObjectives.Any(objective => objective.isCompleted && !objective.isSuccessful))
        {
            OnDefeat?.Invoke(this, EventArgs.Empty);
            return; // Failed objective
        }
        
        if (mainObjectives.Any(objective => !objective.isCompleted && !objective.successOnVictory))
            return; // Uncompleted objective
        
        // Victory
        isVictory = true;
        OnVictory?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Adds a turn to current turn.
    /// </summary>
    private void AddTurn()
    {
        currentTurn++;
        OnNewTurnStart?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Returns objectives in children.
    /// </summary>
    /// <returns></returns>
    private List<Objective> GetObjectives()
    {
        List<Objective> toReturn = new();

        foreach (Transform child in transform)
        {
            Objective objective = child.GetComponent<Objective>();
            toReturn.AddIf(objective, objective);
        }
        
        return toReturn;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Objective_OnObjectiveUpdate(object sender, Objective updatedObjective)
    {
        IsVictoryOrDefeat();
        OnObjectiveUpdate?.Invoke(this, EventArgs.Empty);
    }
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if(startingTeam == _units.units.First().unitTeam)
            AddTurn();
    }
}