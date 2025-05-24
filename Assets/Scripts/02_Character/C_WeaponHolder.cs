using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_WeaponHolder : MonoBehaviour
{
    [SerializeField] private List<WeaponGraphics> weaponGraphicsList;

    [Header("REFERENCES")]

    [SerializeField] private C__Character c;

    private WeaponGraphics currentWeaponGraphics;

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
    /// Returns the current weapon graphics.
    /// </summary>
    /// <returns></returns>
    public WeaponGraphics GetCurrentWeaponGraphics()
    {
        if (currentWeaponGraphics)
            return currentWeaponGraphics;

        foreach (WeaponGraphics testedWeaponGraphics in weaponGraphicsList)
        {
            bool isCurrentWeapon = testedWeaponGraphics.weapon == c.attack.GetCurrentWeapon();

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
            bool isCurrentWeapon = testedWeaponGraphics.weapon == weapon;

            if (isCurrentWeapon)
                currentWeaponGraphics = testedWeaponGraphics;

            testedWeaponGraphics.gameObject.SetActive(isCurrentWeapon);
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
