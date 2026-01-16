using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An inventory for the unit, with a limited size.
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
        int itemsSize = items.Select(testedItem => testedItem.sizeInInventory).Sum();
        
        if(itemsSize + item.sizeInInventory > maxSize)
            return false; // No place for this item
        
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