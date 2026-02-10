using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class Unit_Behavior : MonoBehaviour
{
    [SerializeField] private bool _playable = true;
    public bool playable => _playable;
    
    [SerializeField] private UnitBehavior _behavior;
    [SerializeField] private float _delayBeforeAct = 1f;
    
    [Header("- REFERENCES -")]
    
    [SerializeField] private Unit unit;
    
    private Unit targetUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Start the unit's behavior.
    /// </summary>
    public void PlayBehavior()
    {
        if (_behavior)
        {
            Wait(_delayBeforeAct, TargetEnemy);
            return;
        }

        Wait(1, PassTurn);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Targets the behavior's preferred enemy and choose between attack or move towards.
    /// </summary>
    private void TargetEnemy()
    {
        targetUnit = _behavior.GetPreferredTarget(unit,unit.attack.AttackableTiles()
            .Select(tile => tile.character)
            .ToList());
        
        if(targetUnit)
        {
            if (unit.actionsHolder.HasAvailableAction<A_Attack>())
            {
                Attack();
                return; // Attack unit
            }
            if (unit.actionsHolder.HasAvailableAction<A_Reload>())
            {
                Reload(); // Reload weapon
                return;
            }
        }
        
        targetUnit = _behavior.GetPreferredTarget(unit,unit.look.EnemiesVisibleInFog());

        if (!targetUnit)
        {
            PassTurn();
            return; // No target unit
        }
        
        Tile closestMeleePosition = ClosestMeleePositionOf(targetUnit);
        if (!closestMeleePosition)
        {
            PassTurn();
            return; // Can't reach a melee position
        }
        
        Tile closestTileOfMeleePosition = unit.move.GetFurthestTileTowards(closestMeleePosition);
        
        GoTowards(closestTileOfMeleePosition);
    }
    
    /// <summary>
    /// Reloads the equipped weapon.
    /// </summary>
    private void Reload()
    {
        unit.actionsHolder.GetActionOfType<A_Reload>().StartReload();
    }
    
    /// <summary>
    /// Moves the unit in direction of the target position.
    /// </summary>
    /// <param name="targetPosition"></param>
    private void GoTowards(Tile targetPosition)
    {
        unit.move.MoveTo(targetPosition);
    }
    
    /// <summary>
    /// Attacks the target.
    /// </summary>
    private void Attack()
    {
        unit.attack.StartAttack(targetUnit);
    }
    
    /// <summary>
    /// Passes its turn.
    /// </summary>
    private void PassTurn()
    {
        unit.SetCanPlayValue(false);
        _units.EndCurrentUnitTurn();
    }
    
    /// <summary>
    /// Returns the closest tile of a unit in melee.
    /// </summary>
    /// <param name="meleeUnit"></param>
    /// <returns></returns>
    private Tile ClosestMeleePositionOf(Unit meleeUnit)
    {
        return _board.GetTilesAround(meleeUnit.tile, 1, true)
            .Where(tile => unit.move.CanMoveTowards(tile))
            .OrderBy(tile => Vector3.Distance(tile.transform.position, unit.tile.transform.position))
            .FirstOrDefault(); // Closest tile
    }
    
    /// <summary>
    /// Start a wait for "time" seconds and execute an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));
    
    /// <summary>
    /// Wait for "time" seconds and execute an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}