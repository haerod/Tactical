using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Serialization;

/// <summary>
/// Manages an inventory
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private bool _hasMaxSize = true;
    [SerializeField] private int _maxSize = 6;
    public bool hasMaxSize => _hasMaxSize;
    public int maxSize => _maxSize;
    
    [SerializeField] protected Transform content;
    
    public List<Item> items => GetItems();
    public List<Weapon> weapons => GetWeapons();
    public int remainingSpace => _maxSize - items.Select(testedItem => testedItem.sizeInInventory).Sum();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds the item if it's place enough in the inventory.
    /// Returns true if it works, else returns false.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryAddItem(Item item)
    {
        StackInInventory(item);

        if (!item || item.currentStack == 0)
            return true; // Item fully stacked
        
        if (!_hasMaxSize)
        {
            AddItem(item);
            return true; // No max size
        }
        
        int itemsSize = items.Select(testedItem => testedItem.sizeInInventory).Sum();
        
        if(itemsSize + item.sizeInInventory > maxSize)
            return false; // No place for this item
        
        AddItem(item);
        return true;
    }
    
    /// <summary>
    /// Destroys an item and removes it from inventory.
    /// </summary>
    /// <param name="item"></param>
    public void DestroyItem(Item item)
    {
        item.transform.SetParent(null);
        Destroy(item.gameObject);
    }
    
    /// <summary>
    /// Returns the first weapon corresponding to weapon data.
    /// </summary>
    /// <param name="weaponData"></param>
    /// <returns></returns>
    public Weapon GetWeapon(WeaponData weaponData) => items
        .OfType<Weapon>()
        .FirstOrDefault(weapon => weapon.data == weaponData);
    
    /// <summary>
    /// Returns all ammo of the given Ammo type.
    /// </summary>
    /// <param name="ammoType"></param>
    /// <returns></returns>
    public List<Ammo> GetAmmoOfType(AmmoType ammoType) => items
        .OfType<Ammo>()
        .Where(ammo => ammoType == ammo.ammoType)
        .ToList();
    
    /// <summary>
    /// Returns the number of ammo of the given type.
    /// </summary>
    /// <param name="ammoType"></param>
    /// <returns></returns>
    public int GetAmmoCountOfType(AmmoType ammoType)
    {
        List<Ammo> ammoItems = GetAmmoOfType(ammoType);
        
        if(ammoItems.Count == 0)
            return 0;
        
        return ammoItems.First().stackable ? 
            ammoItems.Select(ammo => ammo.currentStack).Sum() : 
            GetAmmoOfType(ammoType).Count;
    }
    
    /// <summary>
    /// Removes the given count of ammo of this type.
    /// </summary>
    /// <param name="ammoType"></param>
    /// <param name="ammoCountToRemove"></param>
    public void RemoveAmmoOfType(AmmoType ammoType, int ammoCountToRemove)
    {
        List<Item> ammoToRemove = GetAmmoOfType(ammoType)
            .Select(ammo => ammo as Item)
            .ToList();

        int currentAmmoCountToRemove = ammoCountToRemove;
        
        for (int i = 0; i < ammoCountToRemove; i++)
        {
            if (!ammoToRemove[i].stackable)
            {
                DestroyItem(ammoToRemove[i]);
                continue; // Not stackable
            }
            
            currentAmmoCountToRemove -= ammoToRemove[i].TryRemoveCountFromStack(currentAmmoCountToRemove);
            
            if(ammoToRemove[i].stackIsEmpty)
                DestroyItem(ammoToRemove[i]); // Stack is empty
            
            if(currentAmmoCountToRemove == 0)
                return; // Ammo are all removed
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Adds an item in the inventory.
    /// </summary>
    /// <param name="item"></param>
    private void AddItem(Item item) => item.transform.SetParent(content);
    
    /// <summary>
    /// Returns inventory's items.
    /// </summary>
    /// <returns></returns>
    private List<Item> GetItems() => (from Transform child in content select child.GetComponent<Item>()).ToList();
    
    /// <summary>
    /// Returns inventory's weapons.
    /// </summary>
    /// <returns></returns>
    private List<Weapon> GetWeapons() => items
        .OfType<Weapon>()
        .ToList();
    
    /// <summary>
    /// Stacks the item in all the same stackable item of the inventory.
    /// If item is fully stacked, destroys the item.
    /// </summary>
    /// <param name="item"></param>
    private void StackInInventory(Item item)
    {
        if(!item.stackable)
            return; // Not stackable
        
        List<Item> availableStacks = items
            .Where(testedItem => testedItem.itemName == item.itemName && !testedItem.stackIsFull)
            .ToList();

        foreach (Item availableStack in availableStacks)
        {
            int addedCount = availableStack.TryAddCountToStack(item.currentStack);
            item.TryRemoveCountFromStack(addedCount);
            
            if (item.stackIsEmpty)
                DestroyItem(item); // Fully stacked
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}