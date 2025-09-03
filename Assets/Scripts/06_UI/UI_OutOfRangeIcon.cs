using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

/// <summary>
/// UI_OutOfRangeIcon : Shows when the current unit is out of range
/// </summary>
public class UI_OutOfRangeIcon : MonoBehaviour
{
    public GameObject outOfRangeIcon;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Shows the icon.
    /// </summary>
    public void Display() => outOfRangeIcon.SetActive(true);
    
    /// <summary>
    /// Hides the icon.
    /// </summary>
    public void Hide() => outOfRangeIcon.SetActive(false);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
}