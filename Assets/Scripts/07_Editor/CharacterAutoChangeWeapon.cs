using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class CharacterAutoChangeWeapon : MonoBehaviour
{
    private C__Character current;

    private Weapon previousWeapon;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }

        current = GetComponent<C__Character>();

        previousWeapon = current.attack.currentWeapon;
        EditorUtility.SetDirty(this);
    }

    private void Update()
    {
        if (!AreModifications()) 
            return; // No modifications
        
        AutoChangeWeapon();
        UpdateModificationValues();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check for modifications in the current used weapon.
    /// </summary>
    /// <returns></returns>
    private bool AreModifications() => previousWeapon != current.attack.currentWeapon;

    /// <summary>
    /// Update the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousWeapon = current.attack.currentWeapon;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Display the good weapon, hide the other one.
    /// </summary>
    private void AutoChangeWeapon()
    {
        C_WeaponHolder weaponHolder = current.weaponHolder;

        weaponHolder.DisplayWeapon(current.attack.currentWeapon);

        foreach (WeaponGraphics testedWeaponGraphics in weaponHolder.GetWeaponGraphicsList())
        {
            EditorUtility.SetDirty(testedWeaponGraphics.gameObject); // Save the character modifications
        }        
    }
}
