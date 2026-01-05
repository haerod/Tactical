using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class Unit_Inventory : Inventory
{
    [SerializeField] private int maxSize = 6;
    
    [Header("REFERENCES")]
    
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public bool TryAddItem(Item item)
    {
        if(items.Count >= maxSize)
            return false;
        
        item.transform.SetParent(content);

        return true;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================

}