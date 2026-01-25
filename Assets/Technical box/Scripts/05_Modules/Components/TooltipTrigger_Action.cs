using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/// <summary>
/// Trigger to a tooltip's displayer for actions.
/// /// </summary>
public class TooltipTrigger_Action : TooltipTrigger_Basic
{
    [SerializeField] private bool showActionPoints = false;
    [SerializeField] private bool showDescription = true;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private string GetActionDescription()
    {
        string toReturn = "";
        
        A__Action action = GetComponentInParent<Module_ActionSelectionButton>().action;
        
        // Name
        toReturn = $"<b>{action.actionName}</b>";
        // Action points
        if (showActionPoints)
            toReturn += $" ({action.actionPointCost} AP)";
        
        if(showDescription)
            toReturn += $"\n\n <i>{action.actionDescription}</i>";
        
        return toReturn;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    public override void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipHovered(new List<string>() {GetActionDescription()});
    }
}