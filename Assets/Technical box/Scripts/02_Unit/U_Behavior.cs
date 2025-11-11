using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using static M__Managers;

public class U_Behavior : MonoBehaviour
{
    public bool playable = true;
    [SerializeField] private UnitBehavior behavior;
    
    [Header("REFERENCES")]
    
    [SerializeField] private U__Unit unit;
    
    private U__Unit targetUnit;
    
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
        if (!behavior)
        {
            unit.SetCanPlayValue(false);
            Wait(1, () => _units.EndCurrentUnitTurn());
            return;
        }

        Wait(1, TargetEnemy);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Targets the behavior's preferred enemy and choose between attack or move towards.
    /// </summary>
    private void TargetEnemy()
    {
        // if(_rules.IsVictory())
        // {
        //     PassTurn();
        //     return; // Victory
        // }
        
        targetUnit = behavior.GetPreferredTarget(unit,unit.attack.AttackableTiles()
            .Select(tile => tile.character)
            .ToList());
        
        if(targetUnit)
        {
            Attack();
            return; // Attack unit
        }
        
        targetUnit = behavior.GetPreferredTarget(unit,unit.look.EnemiesVisibleInFog());
        
        if (targetUnit)
        {
            Tile closestMeleePosition = ClosestMeleePositionOf(targetUnit);
            if (!closestMeleePosition)
            {
                PassTurn();
                return; // Can't reach a melee position
            }
            
            Tile closestTileOfMeleePosition = unit.move.GetFurthestTileTowards(closestMeleePosition);
            
            GoTowards(closestTileOfMeleePosition);
        }
        
        PassTurn();
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
        unit.attack.Attack(targetUnit);
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
    private Tile ClosestMeleePositionOf(U__Unit meleeUnit)
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
}
