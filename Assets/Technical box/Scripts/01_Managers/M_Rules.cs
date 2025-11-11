using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static M__Managers;

public class M_Rules : MonoBehaviour
{
    [Header("VICTORY RULES")]
    
    public VictoryCondition victoryCondition = VictoryCondition.Deathmatch;
    public enum VictoryCondition { Deathmatch,}
    
    [Header("FOG OF WAR")]
    
    [SerializeField] private FogOfWar fogOfWar;
    public enum VisibleInFogOfWar { InView, Allies, Everybody}
    public VisibleInFogOfWar visibleInFogOfWar = VisibleInFogOfWar.Allies;
    
    public static M_Rules instance => _instance ??= FindFirstObjectByType<M_Rules>();
    public static M_Rules _instance;
    
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
    
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void OnDisable()
    {
        OnVictory = null;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if there is an enabled fog of war.
    /// </summary>
    /// <returns></returns>
    public bool IsFogOfWar() => fogOfWar && fogOfWar.gameObject.activeInHierarchy;

    /// <summary>
    /// Checks if it's currently victory.
    /// </summary>
    public bool IsVictory()
    {
        if (_units.GetEnemiesOf(_units.current).Count > 0)
            return false; // Not victory
        
        OnVictory?.Invoke(null, EventArgs.Empty);
        return true;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}