using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class A_Regen : A__Action
{
    [SerializeField] private int _regenAmount = 1;
    [SerializeField] private GameObject _regenFeedbackPrefab;
    [SerializeField] private float _regenDuration = 1;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Starts the Regen Action
    /// </summary>
    public void StartRegen()
    {
        StartAction();
        Instantiate(_regenFeedbackPrefab, unit.transform.position, Quaternion.identity);
        unit.health.Heal(_regenAmount);
        Wait(_regenDuration, EndRegen);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Ends the Regen action.
    /// </summary>
    private void EndRegen()
    {
        EndAction();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}