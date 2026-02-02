using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;

/// <summary>
/// Spawns an unit.
/// </summary>
public class Spawner : Entity
{
    [Header("- SPAWNER -")] [Space]
    
    [SerializeField] private List<Unit> _randomPool;
    public List<Unit> randomPool => _randomPool;
    // [SerializeField] private bool _destroyOnSpawn;
    [SerializeField] private bool _spawnOnStart = true;
    
    [Header("- GIZMO -")] [Space]
    
    [SerializeField] private Mesh _gizmoMesh; 
    [SerializeField] private Color _gizmoColor = Color.cyan;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawMesh(_gizmoMesh, transform.position, transform.rotation);
    }
    
    private void Start()
    {
        if(_spawnOnStart)
            SpawnRandomUnit();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Spawns the given unit
    /// </summary>
    /// <param name="unitToSpawn"></param>
    public void Spawn(Unit unitToSpawn)
    {
        if(!unitToSpawn)
            return; // No unit to spawn
        
        Unit unit = Instantiate(_randomPool.Randomize().FirstOrDefault());
        unit.MoveAt(GetCoordinates());
        _units.AddUnit(unit);
        
        // if(!_destroyOnSpawn)
        //     return;
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Spawns a random unit of the pool.
    /// </summary>
    public void SpawnRandomUnit()
    {
        if (_randomPool.Count == 0)
            return; // No unit in the pool
        
        Spawn(_randomPool.Randomize().FirstOrDefault());
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private Coordinates GetCoordinates() => new (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}