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

    // * None : pass turn
    // * Follower : follow target, if target
    // * Offensive : find a target and attack it until it doesn't have any action points
    private enum Behavior { None, Follower, Offensive}
    [SerializeField] private Behavior behavior = Behavior.None;
    
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
        switch (behavior)
        {
            case Behavior.None:
                unit.SetCanPlayValue(false);
                Wait(1, () => _units.EndCurrentUnitTurn());
                break;
            case Behavior.Follower:
                Wait(1, () => _units.EndCurrentUnitTurn());
                break;
            case Behavior.Offensive:
                Wait(1, AcquireTarget);
                break;
            default:
                break;
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Choose the closest enemy unit as target.
    /// </summary>
    private void AcquireTarget()
    {
        targetUnit = unit.look.ClosestEnemyOnSight();

        if(!targetUnit || targetUnit.health.IsDead() || _rules.IsVictory()) 
        {
            unit.SetCanPlayValue(false);
            _units.EndCurrentUnitTurn();
            return; // Nobody in sight
        }

        unit.attack.Attack(targetUnit);
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
