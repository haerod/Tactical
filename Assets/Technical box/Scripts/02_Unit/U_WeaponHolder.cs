using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

public class U_WeaponHolder : MonoBehaviour
{
    [SerializeField] private WeaponData currentWeaponData;

    [Header("REFERENCES")]

    [SerializeField] private U__Unit unit;
    [SerializeField] private Transform hand;

    private Weapon currentWeapon;

    public event EventHandler<WeaponData> OnWeaponChange;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        unit.anim.SetWeaponAnimation(GetCurrentWeaponGraphics());
        currentWeapon.Setup(unit);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the current weapon.
    /// </summary>
    /// <returns></returns>
    public WeaponData GetCurrentWeapon() => currentWeaponData;

    /// <summary>
    /// Changes the current weapon to the asked one.
    /// </summary>
    /// <param name="newWeaponData"></param>
    public void SetCurrentWeapon(WeaponData newWeaponData)
    {
        currentWeaponData = newWeaponData;
        DisplayWeapon();
        
        OnWeaponChange?.Invoke(this, currentWeaponData);
    }
    
    /// <summary>
    /// Returns the current weapon graphics.
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeaponGraphics()
    {
        if (currentWeapon)
            return currentWeapon;
        
        foreach (Transform child in hand)
        {
            Weapon weapon = child.GetComponent<Weapon>();
            
            if (!weapon)
                continue;
            
            currentWeapon = weapon;
            return weapon;
        }
        
        return currentWeapon;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current weapon.
    /// </summary>
    private void DisplayWeapon()
    {
        if(!currentWeaponData)
            return; // No weapon
        
        Weapon previousWeapon = hand.GetComponentInChildren<Weapon>();
        if (previousWeapon)
            DestroyPreviousWeapon(previousWeapon);
        
        Weapon weapon = unit.inventory.GetWeapon(currentWeaponData);

        if (!weapon)
        {
            currentWeaponData = null;
            return; // Weapon not available in inventory
        }
        
        GameObject instantiatedWeapon = Instantiate(weapon.gameObject, hand);
        instantiatedWeapon.gameObject.SetActive(true);
        
        unit.anim.SetWeaponAnimation(GetCurrentWeaponGraphics());
    }

    /// <summary>
    /// Destroys the previous weapon instantiated.
    /// </summary>
    /// <param name="previousWeapon"></param>
    private void DestroyPreviousWeapon(Weapon previousWeapon)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode || PrefabStageUtility.GetCurrentPrefabStage())
        {
            DestroyImmediate(previousWeapon.gameObject);
            return;
        }
#endif
        Destroy(previousWeapon.gameObject);
    }
}