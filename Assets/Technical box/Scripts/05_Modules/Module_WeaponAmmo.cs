using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

/// <summary>
/// Class description
/// </summary>
public class Module_WeaponAmmo : MonoBehaviour
{
    [SerializeField] private bool showMag;
    
    [Header("REFERENCES")]
    
    [SerializeField] private UI_SegmentedGaugeClamped ammo;
    [SerializeField] private UI_SegmentedGaugeBasic mag;
    [SerializeField] private Image weaponImage;
    [SerializeField] private GameObject panel;
    
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

    private void Show(WeaponData weaponData)
    {
        weaponImage.sprite = weaponData.GetIcon();

        if (weaponData.IsMeleeWeapon())
        {
            ammo.gameObject.SetActive(false);
            mag.gameObject.SetActive(false);
        }
        else
        {
            ammo.SetMaximumValue(weaponData.GetAmmoByLoader());
            ammo.FillGauge(weaponData.GetAmmoByLoader() - 1);
            ammo.gameObject.SetActive(true);
            
            if(showMag)
                mag.FillGauge(2);
            mag.gameObject.SetActive(showMag);
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
        
        Show(currentUnit.weaponHolder.GetCurrentWeapon());
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
    
    private void WeaponHolder_OnWeaponChange(object sender, WeaponData newWeaponData)
    {
        Show(currentUnit.weaponHolder.GetCurrentWeapon());
    }
}