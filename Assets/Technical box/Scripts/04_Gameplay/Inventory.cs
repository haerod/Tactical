using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// Manages an inventory
/// </summary>
public class Inventory : MonoBehaviour
{
    public List<Item> items => GetItems();
    public List<Weapon> weapons => GetWeapons();
    
    [SerializeField] protected Transform content;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public void AddItem(Item item) => item.transform.SetParent(content);
    
    public void DestroyItem(Item item)
    {
        item.transform.SetParent(null);
        Destroy(item.gameObject);
    }
    
    public Weapon GetWeapon(WeaponData weaponData) => items
        .OfType<Weapon>()
        .FirstOrDefault(weapon => weapon.data == weaponData);
    
    public List<Ammo> GetAmmoOfType(AmmoType ammoType) => items
        .OfType<Ammo>()
        .Where(ammo => ammoType == ammo.ammoType)
        .ToList();

    public int GetAmmoCountOfType(AmmoType ammoType) => GetAmmoOfType(ammoType).Count;
    
    public void RemoveAmmoOfType(AmmoType ammoType, int ammoCountToRemove)
    {
        List<Item> ammoToRemove = GetAmmoOfType(ammoType)
            .Select(ammo => ammo as Item)
            .ToList();

        for (int i = 0; i < ammoCountToRemove; i++)
            DestroyItem(ammoToRemove[i]);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private List<Item> GetItems() => (from Transform child in content select child.GetComponent<Item>()).ToList();
    
    private List<Weapon> GetWeapons() => items
        .OfType<Weapon>()
        .ToList();
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}