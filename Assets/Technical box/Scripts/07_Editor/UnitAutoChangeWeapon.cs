using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class UnitAutoChangeWeapon : MonoBehaviour
{
    private U__Unit current;

    private WeaponData previousWeaponData;

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

        current = GetComponent<U__Unit>();
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
    private bool AreModifications() => previousWeaponData != current.weaponHolder.GetCurrentWeapon() 
                                       && !PrefabStageUtility.GetCurrentPrefabStage();

    /// <summary>
    /// Updates the modification checkers values.
    /// </summary>
    private void UpdateModificationValues()
    {
        previousWeaponData = current.weaponHolder.GetCurrentWeapon();
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Displays the good weapon, hides the other one.
    /// </summary>
    private void AutoChangeWeapon()
    {
        U_WeaponHolder weaponHolder = current.weaponHolder;
        weaponHolder.SetCurrentWeapon(weaponHolder.GetCurrentWeapon());
        
        if(weaponHolder.GetCurrentWeapon())
            EditorUtility.SetDirty(weaponHolder.GetCurrentWeaponGraphics().gameObject); // Save the character modifications
    }
#endif
}
