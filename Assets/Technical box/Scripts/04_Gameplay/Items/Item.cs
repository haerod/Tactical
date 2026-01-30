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
    
    [SerializeField] private string _itemName = "Item name";
    public string itemName => _itemName;
    [SerializeField] private Sprite _icon;
    public Sprite icon => _icon;

    [Space]
    
    [SerializeField] private int _sizeInInventory = 1;
    public int sizeInInventory => _sizeInInventory;
    
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