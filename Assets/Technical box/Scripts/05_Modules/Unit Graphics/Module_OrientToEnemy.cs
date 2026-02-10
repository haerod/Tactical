using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Orients the target to the closest enemy.
/// </summary>
public class Module_OrientToEnemy : MonoBehaviour
{
    [SerializeField] private bool _orientOnStart = true;
    [SerializeField] private bool _orientOnAnyActionEnd = true;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Unit _unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        if(_orientOnStart)
            OrientToClosestEnemy();
        
        if(_orientOnAnyActionEnd)
            GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Orients the target to the closest enemy.
    /// </summary>
    private void OrientToClosestEnemy()
    {
        Unit closestEnemy = _unit.look.ClosestEnemyOnSight();
        
        if(closestEnemy)
            _unit.move.OrientTo(closestEnemy.transform.position);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit e)
    {
        OrientToClosestEnemy();
    }
}