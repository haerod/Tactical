using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public abstract class Objective : MonoBehaviour
{
    [TextArea(0,1000)][SerializeField] protected  string _description;
    public string description => GetDescription() == null ? _description : GetDescription();
    
    [Space]
    
    [SerializeField] protected bool _isOptional;
    [SerializeField] protected bool _successOnVictory;
    
    public bool isOptional => _isOptional;
    public bool successOnVictory => _successOnVictory;
    
    public event EventHandler<Objective> OnObjectiveUpdate;
    
    public bool isCompleted { get; protected set; }
    public bool isSuccessful { get; protected set; }
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    protected void Start()
    {
        if (_successOnVictory)
            isSuccessful = true;
        
        SubscribeToEvents();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE ABSTRACT
    // ======================================================================
    
    /// <summary>
    /// Verifies if the objective is successes or defeats.
    /// </summary>
    protected abstract void CheckObjectiveCompletion();
    
    /// <summary>
    /// Returns the objective description.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetDescription();
    
    /// <summary>
    /// Subscribes to the events (called on Start).
    /// </summary>
    protected abstract void SubscribeToEvents();
    
    /// <summary>
    /// Unsubscribes to the events (called by SuccessObjective and FailObjective)
    /// </summary>
    protected abstract void UnsubscribeFromEvents();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Sets parameters of the objective as a success.
    /// </summary>
    protected void SuccessObjective()
    {
        isCompleted = true;
        isSuccessful = true;
        InvokeOnObjectiveUpdate(this);
        UnsubscribeFromEvents();
    }
    
    /// <summary>
    /// Sets parameters of the objective as a failure.
    /// </summary>
    protected void FailObjective()
    {
        isCompleted = true;
        isSuccessful = false;
        InvokeOnObjectiveUpdate(this);
        UnsubscribeFromEvents();
    }
    
    /// <summary>
    /// Invokes the event OnObjectiveUpdate.
    /// </summary>
    /// <param name="completedObjective"></param>
    protected void InvokeOnObjectiveUpdate(Objective completedObjective) => 
        OnObjectiveUpdate?.Invoke(this, completedObjective);
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
