using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

/// <summary>
/// Class description
/// </summary>
public class Item : MonoBehaviour
{
    [Header("- ITEM -")][Space]
    
    [SerializeField] private string _itemName = "Item name";
    public string itemName => _itemName;
    [SerializeField] private Sprite _icon;
    public Sprite icon => _icon;

    [Space]
    
    [SerializeField] private int _sizeInInventory = 1;
    public int sizeInInventory => _sizeInInventory;
    [SerializeField] private bool _visibleInInventory = true;
    public bool visibleInInventory => _visibleInInventory;
    
    [Header("- ITEM STACK -")][Space]
    
    [SerializeField] private bool _stackable;
    public bool stackable => _stackable;
    [SerializeField] private int _maxStackSize = 1;
    public int maxStackSize => _maxStackSize;
    [SerializeField] private int _currentStack = 1;
    public int currentStack => _currentStack;
    
    public bool stackIsFull => currentStack == maxStackSize;
    public bool stackIsEmpty => currentStack == 0;
    public int remainingSpaceInStack => Mathf.Clamp(maxStackSize - currentStack,  0, maxStackSize);
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds the given count to this item, and returns how much has been added.
    /// </summary>
    /// <param name="countToAdd"></param>
    /// <returns></returns>
    public int TryAddCountToStack(int countToAdd)
    {
        int stackToAdd = Mathf.Clamp(countToAdd, 0, remainingSpaceInStack);
        _currentStack += stackToAdd;
        return stackToAdd;
    }
    
    /// <summary>
    /// Removes the given count to this item, and returns how much has been removed.
    /// </summary>
    /// <param name="countToRemove"></param>
    /// <returns></returns>
    public int TryRemoveCountFromStack(int countToRemove)
    {
        int stackToRemove = Mathf.Clamp(countToRemove, 0, currentStack);
        _currentStack -= stackToRemove;
        return stackToRemove;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
}