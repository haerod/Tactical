using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class Item : MonoBehaviour
{
    [Header("ITEM")]
    
    [SerializeField] private int _sizeInInventory = 1;
    public int sizeInInventory => _sizeInInventory;
    [SerializeField] private string _itemName = "Item name";
    public string itemName => _itemName;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================

}