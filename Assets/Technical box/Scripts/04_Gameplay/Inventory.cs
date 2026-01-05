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
        .FirstOrDefault(weapon => weapon.GetData() == weaponData);

    public List<Weapon> GetWeapons() => items
        .OfType<Weapon>()
        .ToList();
    
    // public List<Item> GetWeaponMagazines (WeaponData weaponData) => items
    //     .Where(item => GetWeapon(weaponData).ammo.itemName == item.itemName)
    //     .ToList();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private List<Item> GetItems() => (from Transform child in content select child.GetComponent<Item>()).ToList();
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}