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
    public event EventHandler OnReloadStart;
    public event EventHandler<Unit> OnReloadEnd;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void StartReload()
    {
        StartAction();
        OnReloadStart?.Invoke(this, EventArgs.Empty);
    }

    public void EndReload()
    {
        unit.weaponHolder.ReloadWeapon(unit.weaponHolder.weapon);
        OnReloadEnd?.Invoke(this, unit);
        EndAction();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    protected override bool IsAvailable()
    {
        Weapon currentWeapon = unit.weaponHolder.weapon;
        WeaponData data = currentWeapon.data;
        
        if(!data.canReload)
            return false; // Weapon can't reload
        if(data.needAmmoToReload && unit.inventory.GetAmmoCountOfType(data.ammoType) <= 0)
            return false; // Not ammo enough
        if(currentWeapon.isFullOfAmmo)
            return false; // Already reloaded
        
        return true;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}