using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
[RequireComponent(typeof(Spawner))]
public class SpawnerAutoSnap : BaseAutoSnap
{
#if UNITY_EDITOR

    private Spawner _spawner;
    private Spawner spawner => _spawner != null ? _spawner : _spawner = GetComponent<Spawner>();
    private List<Unit> _unitsToSpawn = new();
    private List<Unit> unitsToSpawn => _unitsToSpawn.Count != 0 ? _unitsToSpawn : _unitsToSpawn = spawner.randomPool;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // INHERITED
    // ======================================================================
    
    protected override void SetParameters() => transform.SetParent(_Level.transform);

    protected override void MoveObject(Coordinates coordinates)
    {
        transform.position = coordinates.ToVector3();
        spawner.coordinates = coordinates;
    }
    
    protected override bool IsOnValidPosition() => IsWalkableTileUnder() && !IsUnitUnder() && !IsOtherSpawnerOnTile();

    protected override void SetParametersDirty() => EditorUtility.SetDirty(spawner);

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns true if is tile under the spawner, and if all the units can walk on.
    /// </summary>
    /// <returns></returns>
    private bool IsWalkableTileUnder() => Physics.OverlapSphere(transform.position, .1f)
        .Select(collider => collider.GetComponentInParent<Tile>())
        .Any(testedTile => testedTile && unitsToSpawn.All(testedUnit => testedUnit.move.CanWalkOn(testedTile.type)));
    
    /// <summary>
    /// Returns true if is another unit under the spawner.
    /// </summary>
    /// <returns></returns>
    private bool IsUnitUnder() => Physics.OverlapSphere(transform.position, .1f)
            .Select(collider => collider.GetComponentInParent<Unit>())
            .Any(testedUnit => testedUnit != null);
    
    /// <summary>
    /// Returns true if is another spawner at position.
    /// </summary>
    /// <returns></returns>
    private bool IsOtherSpawnerOnTile() => Physics.OverlapSphere(transform.position, .1f)
        .Select(collider => collider.GetComponentInParent<Spawner>())
        .Any(testedSpawner => testedSpawner != null && testedSpawner != spawner);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
#endif
}