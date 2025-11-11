using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A generic gauge can be filled, with a maximum.
/// </summary>
public class UI_SegmentedGaugeClamped : UI_SegmentedGaugeBasic
{
    [SerializeField] private GameObject emptyItemPrefab;
    
    protected int maximumValue;
    
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
    /// Adds Count items to the item parent, clamped to maximum value.
    /// If Count is under Maximum Value, adds empty items to the parent.
    /// /// </summary>
    /// <param name="count"></param>
    protected override void FillGauge(int count)
    {
        DestroyChildren();

        for (int i = 0; i < maximumValue; i++)
        {
            if(i < count)
                Instantiate(itemPrefab, itemParent);
            else
                Instantiate(emptyItemPrefab, itemParent);
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}