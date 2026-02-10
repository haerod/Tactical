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
    [Header("- REFERENCES -")]
    
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _ammoImage;
    [SerializeField] private TextMeshProUGUI _text;
    
    private Unit _currentUnit;
    
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
        _level.OnVictory += Level_OnVictory;
    }
    
    private void OnDisable()
    {
        if(!_currentUnit)
            return;

        _currentUnit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
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
        
        if (!weaponData.usesAmmo)
        {
            _panel.SetActive(false);
            return; // Don't use ammo
        }
        
        _ammoImage.sprite = weaponData.ammo.icon;
        _text.text = weaponData.needAmmoToReload ?
            $"{weapon.currentLoadedAmmo} / {_currentUnit.inventory.GetAmmoCountOfType(weaponData.ammoType).ToString()}" :
            $"{weapon.currentLoadedAmmo}";
        _panel.SetActive(true);
    }
    
    /// <summary>
    /// Hides weapon's info.
    /// </summary>
    private void Hide()
    {
        _panel.SetActive(false);
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

        _currentUnit = startingUnit;
        _currentUnit.weaponHolder.OnWeaponChange += WeaponHolder_OnWeaponChange;
        _currentUnit.weaponHolder.OnReloadWeaponEnd += WeaponHolder_OnReloadWeaponEnd;
        _currentUnit.weaponHolder.weapon.OnAmmoCountChanged += Weapon_OnAmmoCountChanged;
        
        Show(_currentUnit.weaponHolder.weapon);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if (_currentUnit)
        {
            _currentUnit.weaponHolder.OnWeaponChange -= WeaponHolder_OnWeaponChange;
            _currentUnit.weaponHolder.OnReloadWeaponEnd -= WeaponHolder_OnReloadWeaponEnd; 
            _currentUnit.weaponHolder.weapon.OnAmmoCountChanged -= Weapon_OnAmmoCountChanged;
            _currentUnit = null;
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
        
        Show(_currentUnit.weaponHolder.weapon);
    }
    
    private void Level_OnVictory(object sender, Team winningTeam)
    {
        Hide();
    }
}