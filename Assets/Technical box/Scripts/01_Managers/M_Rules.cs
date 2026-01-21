using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static M__Managers;

public class M_Rules : MonoBehaviour
{
    [SerializeField] private Team _playerTeam;
    public Team playerTeam => _playerTeam;
    
    [Header("VICTORY RULES")]
    
    public VictoryCondition victoryCondition = VictoryCondition.Deathmatch;
    public enum VictoryCondition { Deathmatch, ReachZone}

    [SerializeField] private List<Tile> _tilesToReach;
    public List<Tile> tilesToReach => _tilesToReach;
    
    [Header("FOG OF WAR")]
    
    [SerializeField] private FogOfWar fogOfWar;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    public static M_Rules instance => _instance == null ? FindFirstObjectByType<M_Rules>() : _instance;
    public static M_Rules _instance;
    
    public bool isFogOfWar => fogOfWar && fogOfWar.gameObject.activeInHierarchy;
    public bool isVictory => IsVictory();
    
    public event EventHandler OnVictory;
    
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
    }

    private void Start()
    {
        if (victoryCondition == VictoryCondition.ReachZone)
            GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
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
        if(victoryCondition ==  VictoryCondition.Deathmatch)
            if (_units.GetEnemiesOf(_units.current).Count > 0)
                return false; // No victory
        
        if (victoryCondition ==  VictoryCondition.ReachZone)
            if(!_units.GetUnitsOf(playerTeam).Any(unit => _tilesToReach.Contains(unit.tile)))
                return false; // No victory
        
        OnVictory?.Invoke(null, EventArgs.Empty);
        return true;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void GameEvents_OnAnyActionEnd(object sender, Unit endingUnit)
    {
        IsVictory();
    }
}