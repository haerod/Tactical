using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// A generic gauge can be filled, with a maximum.
/// </summary>
public class UI_SegmentedGaugeClamped : UI_SegmentedGaugeBasic
{
    [SerializeField] protected Sprite icon;
    [SerializeField] protected Color baseItemColor = Color.white;
    [SerializeField] protected Color emptyItemColor = Color.gray;
    
    protected int maximumValue;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Sets the maximum value to the given one.
    /// </summary>
    /// <param name="value"></param>
    public void SetMaximumValue(int value) => maximumValue = value;
    
    /// <summary>
    /// Changes the printed icon.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetIcon(Sprite sprite) => icon = sprite;
    
    /// <summary>
    /// Adds Count items to the item parent, clamped to maximum value.
    /// If Count is under Maximum Value, adds empty items to the parent.
    /// /// </summary>
    /// <param name="count"></param>
    public override void FillGauge(int count)
    {
        DestroyChildren();
        
        for (int i = 0; i < maximumValue; i++)
        {
            GameObject instantiatedItem = Instantiate(itemPrefab, itemParent);
            Image image = instantiatedItem.GetComponent<Image>();
            image.color = baseItemColor;
            image.sprite = icon;
            
            if(i < count)
                continue; // Not the last
            
            image.color = emptyItemColor;
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}