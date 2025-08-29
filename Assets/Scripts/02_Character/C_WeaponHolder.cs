using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_WeaponHolder : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private List<Weapon> unitWeapons;

    [Header("REFERENCES")]

    [SerializeField] private C__Character c;
    [SerializeField] private List<WeaponGraphics> weaponGraphicsList;

    private WeaponGraphics currentWeaponGraphics;

    public event EventHandler<Weapon> OnWeaponChange;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    private void Start()
    {
        c.anim.SetWeaponAnimation(c.weaponHolder.GetCurrentWeaponGraphics());
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

        foreach (WeaponGraphics testedWeaponGraphics in weaponGraphicsList)
        {
            bool isCurrentWeapon = testedWeaponGraphics.GetWeapon() == currentWeapon;

            if (isCurrentWeapon)
                currentWeaponGraphics = testedWeaponGraphics;
        }

        return currentWeaponGraphics;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the asked weapon, hide the other one.
    /// </summary>
    private void DisplayWeapon()
    {
        foreach (WeaponGraphics testedWeaponGraphics in weaponGraphicsList)
        {
            bool isCurrentWeapon = testedWeaponGraphics.GetWeapon() == currentWeapon;

            if (isCurrentWeapon)
                currentWeaponGraphics = testedWeaponGraphics;

            testedWeaponGraphics.gameObject.SetActive(isCurrentWeapon);
        }
        
        c.anim.SetWeaponAnimation(GetCurrentWeaponGraphics());
    }

}
