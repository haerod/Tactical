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
    
    [SerializeField] private Weapon _weapon;
    public WeaponData weaponData => weapon.data;
    public Weapon weapon => _weapon;
    public Weapon weaponGraphics => _weaponGraphics;
    
    [Header("REFERENCES")]
    
    [SerializeField] private U__Unit unit;
    [SerializeField] private Transform hand;
    [SerializeField] private Weapon _weaponGraphics;
    
    public event EventHandler<Weapon> OnWeaponChange;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        unit.anim.SetWeaponAnimation(weapon);
        if(weaponGraphics)
            weaponGraphics.Setup(unit);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Changes the weapon to the given one.
    /// </summary>
    /// <param name="newWeapon"></param>
    public void EquipWeapon(Weapon newWeapon)
    {
        _weapon = newWeapon;
        DisplayWeapon();
        
        OnWeaponChange?.Invoke(this, weapon);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current weapon.
    /// </summary>
    private void DisplayWeapon()
    {
        Weapon previousWeapon = hand.GetComponentInChildren<Weapon>();
        if (previousWeapon)
            DestroyPreviousWeapon(previousWeapon);
        
        Weapon weaponToDisplay = unit.inventory.GetWeapon(weaponData);
        if (!weaponToDisplay)
            return; // Weapon not available in inventory
        
        GameObject instantiatedWeapon = Instantiate(weaponToDisplay.gameObject, hand);
        instantiatedWeapon.gameObject.SetActive(true);
        _weaponGraphics = instantiatedWeapon.GetComponent<Weapon>();

        weaponGraphics.Setup(unit);
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