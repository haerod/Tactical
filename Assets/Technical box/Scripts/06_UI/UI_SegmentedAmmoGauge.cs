using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Class description
/// </summary>
public class UI_SegmentedAmmoGauge : UI_SegmentedGaugeClamped
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Adds Count items to the item parent, clamped to maximum value.
    /// If Count is under Maximum Value, adds empty items to the parent.
    /// /// </summary>
    /// <param name="count"></param>
    public void FillGauge(int count, Sprite icon)
    {
        DestroyChildren();

        for (int i = 0; i < maximumValue; i++)
        {
            GameObject instantiatedItem = Instantiate(itemPrefab, itemParent);

            if (i < count) 
                continue; // Not an empty icon
            
            Image image = instantiatedItem.GetComponent<Image>();
            image.color = emptyItemColor;
            image.sprite = icon;
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================

}