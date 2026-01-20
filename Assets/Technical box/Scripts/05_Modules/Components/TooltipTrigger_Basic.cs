using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// Trigger to a tooltip's displayer.
/// </summary>
public class TooltipTrigger_Basic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    [SerializeField] private List<string> content;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void OnDisable()
    {
        GameEvents.InvokeOnAnyTooltipExit();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipHovered(content);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyTooltipExit();
    }
}