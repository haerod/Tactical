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
    
    [SerializeField] private UI_SegmentedAmmoGauge ammo;
    [SerializeField] private Image weaponImage;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject remainingAmmoPanel;
    [SerializeField] private TextMeshProUGUI remainingAmmoText;
    [SerializeField] private Button reloadButton;
    
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        Hide();
    }
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionStart += Action_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
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
    
    /// <summary>
    /// Used by Reload Weapon Button
    /// </summary>
    public void ClickOnReloadWeaponButton() => currentUnit.actionsHolder.GetActionOfType<A_Reload>().StartReload();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays weapon's info on UI.
    /// </summary>
    /// <param name="weapon"></param>
    private void Show(Weapon weapon)
    {
        WeaponData weaponData = weapon.data;
        weaponImage.sprite = weapon.icon;

        if (weaponData.usesAmmo)
        {
            ammo.SetMaximumValue(weaponData.ammoCount);
            ammo.FillGauge(weapon.currentLoadedAmmo, weaponData.ammo.icon);
            ammo.gameObject.SetActive(true);
            remainingAmmoPanel.gameObject.SetActive(weaponData.needAmmoToReload);
            
            if (currentUnit.actionsHolder.HasAction<A_Reload>() && weaponData.canReload && !weapon.isFullOfAmmo)
            {
                reloadButton.gameObject.SetActive(true);
                reloadButton.interactable = currentUnit.actionsHolder.HasAvailableAction<A_Reload>();
            }
            else
            {
                reloadButton.gameObject.SetActive(false);
            }
                
            if (weaponData.needAmmoToReload)
                remainingAmmoText.text = currentUnit.inventory.GetAmmoCountOfType(weaponData.ammoType).ToString();
        }
        else
        {
            ammo.gameObject.SetActive(false);
            remainingAmmoPanel.SetActive(false);
            reloadButton.gameObject.SetActive(false);
        }
        
        panel.SetActive(true);
    }
    
    /// <summary>
    /// Hides weapon's info.
    /// </summary>
    private void Hide()
    {
        panel.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.CanPlay())
            return; // Can't play
        if(!startingUnit.behavior.playable)
            return; // Not playable character

        currentUnit = startingUnit;
        currentUnit.weaponHolder.OnWeaponChange += WeaponHolder_OnWeaponChange;
        currentUnit.weaponHolder.OnReloadWeaponEnd += WeaponHolder_OnReloadWeaponEnd;
        currentUnit.weaponHolder.weapon.OnAmmoCountChanged += Weapon_OnAmmoCountChanged;
        
        Show(currentUnit.weaponHolder.weapon);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if (currentUnit)
        {
            currentUnit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
            currentUnit.weaponHolder.OnReloadWeaponEnd -= WeaponHolder_OnReloadWeaponEnd; 
            currentUnit.weaponHolder.weapon.OnAmmoCountChanged -= Weapon_OnAmmoCountChanged;
            currentUnit = null;
        }
        
        Hide();
    }
    
    private void WeaponHolder_OnWeaponChange(object sender, Unit_WeaponHolder.WeaponChangeEventArgs args)
    {
        args.previousWeapon.OnAmmoCountChanged -= Weapon_OnAmmoCountChanged;
        args.newWeapon.OnAmmoCountChanged += Weapon_OnAmmoCountChanged;
        Show(args.newWeapon);
    }
    
    private void Weapon_OnAmmoCountChanged(object sender, Weapon weapon)
    {
        Show(weapon);
    }
    
    private void WeaponHolder_OnReloadWeaponEnd(object sender, Weapon weapon)
    {
        Show(weapon);
    }
    
    private void Action_OnAnyActionStart(object sender, Unit startingActionUnit)
    {
        Hide();
    }
    
    private void Action_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if (!endingActionUnit.behavior.playable) 
            return; // NPC
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        
        Show(currentUnit.weaponHolder.weapon);
    }
}