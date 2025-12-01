using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A generic gauge can be filled, without maximum.
/// </summary>
public class UI_SegmentedGaugeBasic : MonoBehaviour
{
    [SerializeField] protected Transform itemParent;
    [SerializeField] protected GameObject itemPrefab;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds Count items to the item parent.
    /// </summary>
    /// <param name="count"></param>
    public virtual void FillGauge(int count)
    {
        DestroyChildren();

        for (int i = 0; i < count; i++)
        {
            Instantiate(itemPrefab, itemParent);
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Destroys all items of the item parent.
    /// </summary>
    protected void DestroyChildren()
    {
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }
    }

    // ======================================================================
    // EVENTS
    // ======================================================================

}