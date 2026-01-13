using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class Unit_WeaponHolder : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    public WeaponData weaponData => weapon.data;
    public Weapon weapon => _weapon;
    
    // DEFINITIONS
    // weapon = object in the inventory : is the used weapon
    // weaponData = data of this weapon
    // weaponGraphics = instantiated object in the hand : only visuals, don't use its datas or pararmeters out of that
    
    [Header("BEHAVIOR")]
    
    [SerializeField] private bool _loadWeaponsOnStart = true;

    [Header("REFERENCES")]
    
    [SerializeField] private U__Unit unit;
    [SerializeField] private Transform hand;
    [SerializeField] private Weapon _weaponGraphics;
    public Weapon weaponGraphics => _weaponGraphics;
    
    public event EventHandler<WeaponChangeEventArgs> OnWeaponChange;
    public class WeaponChangeEventArgs : EventArgs
    {
        public Weapon previousWeapon;
        public Weapon newWeapon;
    }

    public event EventHandler<Weapon> OnReloadWeaponEnd;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        if(unit.attack) // Fix for the ragdoll
            unit.attack.OnAttackStart += Attack_OnAttackStart;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        
        unit.anim.SetWeaponAnimation(weapon);
        
        if(weaponGraphics)
            weaponGraphics.Setup(unit);

        if (_loadWeaponsOnStart)
            unit.inventory.weapons.ForEach(ReloadWeapon);
    }
    
    private void OnDisable()
    {
        if(unit.attack) // Fix for the ragdoll
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
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        if(weaponData.usesAmmo)
            weapon.SpendAmmo();
    }
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(startingUnit != unit)
            return; // Not current unit
        
        _input.OnReloadWeaponInput += Input_OnReloadWeaponInput;
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(endingUnit != unit)
            return; // Not current unit
        
        _input.OnReloadWeaponInput -= Input_OnReloadWeaponInput;
    }
    
    private void Input_OnReloadWeaponInput(object sender, EventArgs e)
    {
        if(!unit.behavior.playable)
            return; // NPC
        
        ReloadWeapon(weapon);
    }
}