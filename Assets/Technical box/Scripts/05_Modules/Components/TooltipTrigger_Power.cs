using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class TooltipTrigger_Power : TooltipTrigger_Basic
{
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
    private string GetPowerDescription()
    {
        Power power = GetComponent<Module_PowerDisplayerIcon>().power;
        return $"<b>{power.powerName}</b>" + $"\n\n{power.description}";
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipHovered(new List<string>() {GetPowerDescription()});
    }
}