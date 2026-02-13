using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class Unit_WeaponHolder : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    public Weapon weapon => _weapon;
    public WeaponData weaponData => weapon.data;
    
    // DEFINITIONS
    // weapon = object in the inventory : is the used weapon
    // weaponData = data of this weapon
    // weaponGraphics = instantiated object in the hand : only visuals, don't use its data or parameters out of that
    
    [Header("- SETTINGS -")]
    
    [SerializeField] private bool _loadWeaponsOnStart = true;
    
    [Header("- REFERENCES -")]
    
    [SerializeField] private Unit unit;
    [SerializeField] private Weapon _weaponGraphics;
    public Weapon weaponGraphics => _weaponGraphics;
    
    public event EventHandler<WeaponChangeEventArgs> OnWeaponChange;
    public class WeaponChangeEventArgs : EventArgs { public Weapon previousWeapon, newWeapon;}
    
    public event EventHandler<Weapon> OnReloadWeaponEnd;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        unit.attack.OnAttackStart += Attack_OnAttackStart;
        
        if(weaponGraphics)
            weaponGraphics.Setup(unit);
        
        if (_loadWeaponsOnStart)
            unit.inventory.weapons.ForEach(ReloadWeapon);
    }
    
    private void OnDisable()
    {
        unit.attack.OnAttackStart -= Attack_OnAttackStart;
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
        Weapon previousWeapon = weapon;
        _weapon = newWeapon;
        DisplayWeapon();
        
        OnWeaponChange?.Invoke(this, new WeaponChangeEventArgs{previousWeapon = previousWeapon, newWeapon = weapon});
    }
    
    /// <summary>
    /// Reloads the weapon with ammo if it can.
    /// </summary>
    public void ReloadWeapon(Weapon weaponToReload)
    {
        WeaponData weaponToReloadData = weaponToReload.data;
        
        if(!weaponToReloadData.usesAmmo)
            return; // Don't uses ammo
        if(!weaponToReloadData.canReload)
            return; // Can't be reloaded
        
        if (weaponToReloadData.needAmmoToReload)
        {
            int usedAmmo = weaponToReload.ReloadWeaponOf(unit.inventory.GetAmmoCountOfType(weaponToReloadData.ammoType));
            unit.inventory.RemoveAmmoOfType(weaponToReloadData.ammoType, usedAmmo);
        }
        else
            weaponToReload.ReloadWeaponOf(weaponToReload.AmmoCountToFullyReload());
        
        OnReloadWeaponEnd?.Invoke(this, weaponToReload);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current weapon.
    /// </summary>
    private void DisplayWeapon()
    {
        Transform hand = unit.graphics.rightHand;
        Weapon previousWeapon = hand.GetComponentInChildren<Weapon>();
        if (previousWeapon)
            DestroyPreviousWeapon(previousWeapon);
        
        Weapon weaponToDisplay = unit.inventory.GetWeapon(weaponData);
        if (!weaponToDisplay)
            return; // Weapon not available in inventory
        
        GameObject instantiatedWeapon = Instantiate(weaponToDisplay.gameObject, hand);
        instantiatedWeapon.transform.rotation *= Quaternion.Euler(unit.graphics.handRotationOffset);
        instantiatedWeapon.transform.localPosition += unit.graphics.handPositionOffset;
        instantiatedWeapon.gameObject.SetActive(true);
        _weaponGraphics = instantiatedWeapon.GetComponent<Weapon>();
        
        weaponGraphics.ShowGraphics();
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
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        if(weaponData.usesAmmo)
            weapon.SpendAmmo();
    }
}