using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_WeaponHolder : MonoBehaviour
{
    [SerializeField] private List<WeaponGraphics> weaponGraphicsList;

    [Header("REFERENCES")]

    [SerializeField] private C__Character c;

    private WeaponGraphics currentWeaponGraphics;

    public event EventHandler<WeaponGraphics> OnWeaponChange;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the weapon graphic list.
    /// </summary>
    /// <returns></returns>
    public List<WeaponGraphics> GetWeaponGraphicsList() => weaponGraphicsList;

    /// <summary>
    /// Returns all the weapons as a list.
    /// </summary>
    /// <returns></returns>
    public List<Weapon> GetWeaponList() => GetWeaponGraphicsList()
        .Select(w => w.GetWeapon())
        .ToList();
    
    /// <summary>
    /// Returns the current weapon graphics.
    /// </summary>
    /// <returns></returns>
    public WeaponGraphics GetCurrentWeaponGraphics()
    {
        if (currentWeaponGraphics)
            return currentWeaponGraphics;

        foreach (WeaponGraphics testedWeaponGraphics in weaponGraphicsList)
        {
            bool isCurrentWeapon = testedWeaponGraphics.GetWeapon() == c.attack.GetCurrentWeapon();

            if (isCurrentWeapon)
                currentWeaponGraphics = testedWeaponGraphics;
        }

        return currentWeaponGraphics;
    }
    
    /// <summary>
    /// Displays the asked weapon, hide the other one.
    /// </summary>
    /// <param name="weapon"></param>
    public void DisplayWeapon(Weapon weapon)
    {
        foreach (WeaponGraphics testedWeaponGraphics in weaponGraphicsList)
        {
            bool isCurrentWeapon = testedWeaponGraphics.GetWeapon() == weapon;

            if (isCurrentWeapon)
                currentWeaponGraphics = testedWeaponGraphics;

            testedWeaponGraphics.gameObject.SetActive(isCurrentWeapon);
        }
        
        OnWeaponChange?.Invoke(this, currentWeaponGraphics);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
