using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Unit_PowersHolder : MonoBehaviour
{
    public List<Power> powers => GetPowers();
    
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
    /// Returns unit's powers.
    /// </summary>
    /// <returns></returns>
    private List<Power> GetPowers() => (from Transform child in transform select child.GetComponent<Power>()).ToList();
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
}