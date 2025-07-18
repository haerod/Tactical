using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class WeaponGraphics : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private RuntimeAnimatorController weaponAnimatorController;

    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the animator controller corresponding to the weapon.
    /// </summary>
    /// <returns></returns>
    public RuntimeAnimatorController GetWeaponAnimatorController() => weaponAnimatorController;

    /// <summary>
    /// Returns the current weapon.
    /// </summary>
    /// <returns></returns>
    public Weapon GetWeapon() => weapon;
    
    /// <summary>
    /// Returns the weapon's muzzle flash.
    /// </summary>
    /// <returns></returns>
    public GameObject GetMuzzleFlash() => muzzleFlash;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
