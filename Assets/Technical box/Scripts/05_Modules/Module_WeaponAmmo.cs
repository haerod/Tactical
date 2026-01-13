using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_WeaponAmmo : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private UI_SegmentedGaugeClamped ammo;
    [SerializeField] private Image weaponImage;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject remainingAmmoPanel;
    [SerializeField] private TextMeshProUGUI remainingAmmoText;
    
    private U__Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }

    private void OnDisable()
    {
        if(!currentUnit)
            return;

        currentUnit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Show(Weapon weapon)
    {
        WeaponData weaponData = weapon.data;
        weaponImage.sprite = weaponData.icon;

        if (weaponData.isMeleeWeapon)
        {
            ammo.gameObject.SetActive(false);
            remainingAmmoPanel.SetActive(false);
        }
        else
        {
            ammo.SetMaximumValue(weaponData.ammoCount);
            ammo.FillGauge(weaponData.ammoCount - 1);
            ammo.gameObject.SetActive(true);
            remainingAmmoPanel.gameObject.SetActive(weaponData.needAmmoToReload);
            
            if (weaponData.needAmmoToReload)
                remainingAmmoText.text = "8";
        }
        
        panel.SetActive(true);
    }

    private void Hide()
    {
        panel.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.CanPlay())
            return; // Can't play
        if(!startingUnit.behavior.playable)
            return; // Not playable character

        currentUnit = startingUnit;
        currentUnit.weaponHolder.OnWeaponChange += WeaponHolder_OnWeaponChange;
        
        Show(currentUnit.weaponHolder.weapon);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if (currentUnit)
        {
            currentUnit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
            currentUnit = null;
        }
        
        Hide();
    }
    
    private void WeaponHolder_OnWeaponChange(object sender, Weapon weapon)
    {
        Show(weapon);
    }
}