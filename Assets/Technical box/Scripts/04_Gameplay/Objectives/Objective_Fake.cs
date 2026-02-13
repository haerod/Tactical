using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Objective_Fake : Objective
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

    protected override void CheckObjectiveCompletion()
    {
    }

    protected override string GetDescription() => _description;

    protected override void SubscribeToEvents()
    {
    }

    protected override void UnsubscribeFromEvents()
    {
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}