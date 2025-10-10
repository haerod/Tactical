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
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private List<Weapon> unitWeapons;

    [Header("REFERENCES")]

    [SerializeField] private U__Unit unit;
    [SerializeField] private List<WeaponGraphics> weaponGraphicsList;
    [SerializeField] private Transform hand;

    private WeaponGraphics currentWeaponGraphics;

    public event EventHandler<Weapon> OnWeaponChange;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        unit.anim.SetWeaponAnimation(GetCurrentWeaponGraphics());
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the current weapon.
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon() => currentWeapon;

    /// <summary>
    /// Changes the current weapon to the asked one.
    /// </summary>
    /// <param name="newWeapon"></param>
    public void SetCurrentWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        DisplayWeapon();
        
        OnWeaponChange?.Invoke(this, currentWeapon);
    }

    /// <summary>
    /// Returns the weapon graphic list.
    /// </summary>
    /// <returns></returns>
    public List<WeaponGraphics> GetWeaponGraphicsList() => weaponGraphicsList;

    /// <summary>
    /// Returns all the weapons as a list.
    /// </summary>
    /// <returns></returns>
    public List<Weapon> GetWeaponList() => unitWeapons;
    
    /// <summary>
    /// Returns the current weapon graphics.
    /// </summary>
    /// <returns></returns>
    public WeaponGraphics GetCurrentWeaponGraphics()
    {
        if (currentWeaponGraphics)
            return currentWeaponGraphics;

        foreach (Transform child in hand)
        {
            WeaponGraphics weaponGraphics = child.GetComponent<WeaponGraphics>();
            
            if (!weaponGraphics)
                continue;

            currentWeaponGraphics = weaponGraphics;
            return weaponGraphics;
        }

        return currentWeaponGraphics;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the current weapon.
    /// </summary>
    private void DisplayWeapon()
    {
        if (currentWeaponGraphics)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                Destroy(currentWeaponGraphics.gameObject);
            else 
                DestroyImmediate(currentWeaponGraphics.gameObject);
        }
        
        if(!currentWeapon)
            return; // No weapon
        
        GameObject weaponPrefab = weaponGraphicsList.First(weapon => weapon.GetWeapon() == currentWeapon).gameObject;
        currentWeaponGraphics = Instantiate(weaponPrefab, hand).GetComponent<WeaponGraphics>();
        unit.anim.SetWeaponAnimation(GetCurrentWeaponGraphics());
    }
}