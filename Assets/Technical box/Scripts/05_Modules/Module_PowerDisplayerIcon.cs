using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Module_PowerDisplayerIcon : MonoBehaviour
{
    [SerializeField] private Image _icon;
    
    public Power power { get; private set; }
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Setups the power icon.
    /// </summary>
    /// <param name="_newPower"></param>
    public void Initialize(Power _newPower)
    {
        power = _newPower;
        _icon.sprite = power.icon;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
}