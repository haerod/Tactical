using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static M__Managers;

public class M_Level : MonoBehaviour
{
    [SerializeField] private Team _playerTeam;
    public Team playerTeam => _playerTeam;
    
    [Header("- VICTORY CONDITIONS -")] [Space]
    
    public VictoryCondition victoryCondition = VictoryCondition.Deathmatch;
    public enum VictoryCondition { Deathmatch, ReachZone, Survive}
    
    [SerializeField] private List<Tile> _tilesToReach;
    public List<Tile> tilesToReach => _tilesToReach;
    
    [Space] 
    
    [SerializeField] private int _turnsToSurvive;
    public int turnsToSurvive => _turnsToSurvive;
    
    [Header("- FOG OF WAR -")] [Space]
    
    [SerializeField] private FogOfWar _fogOfWar;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    private static M_Level _instance;
    public static M_Level instance => _instance == null ? _instance = FindFirstObjectByType<M_Level>() : _instance;
        
    public bool isFogOfWar => _fogOfWar && _fogOfWar.gameObject.activeInHierarchy;
    public bool isVictory => IsVictory();
    public int currentTurn {get; private set;}
    
    public event EventHandler<Team> OnVictory;
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
        if (victoryCondition == VictoryCondition.ReachZone)
            GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
        
        if (victoryCondition == VictoryCondition.Survive)
            _units.OnTeamTurnStart += Units_OnTeamTurnStart;
    }
    
    private void OnDisable()
    {
        OnVictory = null;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    private bool IsVictory()
    {
        if (victoryCondition == VictoryCondition.Deathmatch)
        {
            if (_units.GetEnemiesOf(_units.current).Count == 0)
                OnVictory?.Invoke(null, _units.current.unitTeam);
            else
                return false;
        }
        
        if (victoryCondition == VictoryCondition.ReachZone)
        {
            if(_units.GetUnitsOf(playerTeam).Count == 0)
                OnVictory?.Invoke(null, _units.units.FirstOrDefault()?.unitTeam);
            else if (_units.GetUnitsOf(playerTeam).Any(unit => _tilesToReach.Contains(unit.tile)))
                OnVictory?.Invoke(null, playerTeam);
            else
                return false;
        }
        
        if (victoryCondition == VictoryCondition.Survive)
        {
            if(_units.GetUnitsOf(playerTeam).Count == 0)
                OnVictory?.Invoke(null, _units.units.FirstOrDefault()?.unitTeam);
            else if (currentTurn == turnsToSurvive)
                OnVictory?.Invoke(null, playerTeam);
            else
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Adds a turn to current turn.
    /// </summary>
    private void AddTurn()
    {
        currentTurn++;
        OnNewTurnStart?.Invoke(this, EventArgs.Empty);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingUnit)
    {
        IsVictory();
    }
    
    private void Units_OnTeamTurnStart(object sender, Team startingTeam)
    {
        if(startingTeam != playerTeam)
            return; // Not player team
    
        AddTurn();
        IsVictory();
    }
}