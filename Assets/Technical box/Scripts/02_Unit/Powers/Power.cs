using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public abstract class Power : MonoBehaviour
{
    [SerializeField] protected string _powerName = "Power name";
    [SerializeField] protected Sprite _icon;
    [TextArea(0,1000)][SerializeField] protected string _description = "Description in UI";
    [TextArea(0,1000)][SerializeField] protected string _textFeedback = "Text feedback on applied";
    [SerializeField] protected Color _textFeedbackColor = Color.white;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] protected Unit _unit;
    
    public string powerName => _powerName;
    public Sprite icon => _icon;
    public string description => GetDescription();
    public string textFeedback => GetTextFeedback();
    public Unit unit => _unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Invokes the event.
    /// </summary>
    protected void ExecutePower() => GameEvents.InvokeOnAnyPowerExecute(this);
    
    /// <summary>
    /// Returns the description for tooltips.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetDescription();
    
    /// <summary>
    /// Returns the description for text feedback.
    /// </summary>
    /// <returns></returns>
    protected abstract string GetTextFeedback();
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}