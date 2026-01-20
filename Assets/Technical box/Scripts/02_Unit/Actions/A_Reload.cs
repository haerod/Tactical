using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class A_Reload : A__Action
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

    public void Reload()
    {
        StartAction();
        unit.weaponHolder.ReloadWeapon(unit.weaponHolder.weapon);
        EndAction();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}