using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Trigger to a tooltip's displayer for weapon's images.
/// /// </summary>
public class TooltipTrigger_Item : TooltipTrigger_Basic
{

    [SerializeField] private bool _showSize = true;
    [SerializeField] private bool _showStack = true;
    
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
    /// Returns the item's description.
    /// </summary>
    /// <returns></returns>
    private string GetItemDescription()
    {
        string toReturn = "";

        Item item = GetComponentInParent<Module_InventoryDisplayerButton>().item;
        
        // Name
        toReturn = $"<b>{item.itemName}</b>";

        if (_showSize)
        {
            toReturn += $" ({item.sizeInInventory} slot";
            if (item.sizeInInventory > 1)
                toReturn += $"s";
            toReturn += $")";
        }
        if (_showStack && item.stackable)
            toReturn += $"\n{item.currentStack}/{item.maxStackSize}";
        
        return toReturn;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    public override void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipHovered(new List<string>() {GetItemDescription()});
    }
}