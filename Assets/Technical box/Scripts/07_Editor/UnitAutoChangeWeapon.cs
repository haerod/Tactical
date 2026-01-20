using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class UnitAutoChangeWeapon : MonoBehaviour
{
    private Unit current;

    private Weapon previousWeapon;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR
    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }

        current = GetComponent<Unit>();
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
    private bool AreModifications() => previousWeapon != current.weaponHolder.weapon 
                                       && !PrefabStageUtility.GetCurrentPrefabStage();

    /// <summary>
    /// Updates the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousWeapon = current.weaponHolder.weapon;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Displays the good weapon, hides the other one.
    /// </summary>
    private void AutoChangeWeapon()
    {
        Unit_WeaponHolder weaponHolder = current.weaponHolder;
        weaponHolder.EquipWeapon(weaponHolder.weapon);
        
        if(weaponHolder.weapon)
            EditorUtility.SetDirty(weaponHolder); // Save the character modifications
    }
#endif
}
