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

        previousWeapon = current.weaponHolder.GetCurrentWeapon();
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
    /// Checks for modifications in the current used weapon.
    /// </summary>
    /// <returns></returns>
    private bool AreModifications() => previousWeapon != current.weaponHolder.GetCurrentWeapon();

    /// <summary>
    /// Updates the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousWeapon = current.weaponHolder.GetCurrentWeapon();
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Displays the good weapon, hides the other one.
    /// </summary>
    private void AutoChangeWeapon()
    {
        C_WeaponHolder weaponHolder = current.weaponHolder;

        weaponHolder.SetCurrentWeapon(weaponHolder.GetCurrentWeapon());

        foreach (WeaponGraphics testedWeaponGraphics in weaponHolder.GetWeaponGraphicsList())
        {
            EditorUtility.SetDirty(testedWeaponGraphics.gameObject); // Save the character modifications
        }
    }
}
