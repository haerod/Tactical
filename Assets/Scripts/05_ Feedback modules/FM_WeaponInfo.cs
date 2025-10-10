using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using static M__Managers;
using Button = UnityEngine.UI.Button;

/// <summary>
/// Class description
/// </summary>
public class FM_WeaponInfo : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI weaponDamageTypeText;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        FM_WeaponSelectionButton.OnAnyWeaponSelectionButtonEnter += WeaponSelectionButton_OnAnyWeaponSelectionButtonEnter;
        FM_WeaponSelectionButton.OnAnyWeaponSelectionButtonExit += WeaponSelectionButton_OnAnyWeaponSelectionButtonExit;
    }

    private void OnDisable()
    {
        FM_WeaponSelectionButton.OnAnyWeaponSelectionButtonEnter -= WeaponSelectionButton_OnAnyWeaponSelectionButtonEnter;
        FM_WeaponSelectionButton.OnAnyWeaponSelectionButtonExit -= WeaponSelectionButton_OnAnyWeaponSelectionButtonExit;        
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Shows the feedback.
    /// </summary>
    /// <param name="weaponSelectionButton"></param>
    private void Display(FM_WeaponSelectionButton weaponSelectionButton)
    {
        Weapon weapon = weaponSelectionButton.GetWeapon();
        
        // Name
        weaponNameText.text = weapon.GetName();
        
        // Damage range
        Vector2Int weaponDamageRange = weapon.GetDamagesRange();
        if (weaponDamageRange.x != weaponDamageRange.y)
            weaponDamageText.text = $"{weaponDamageRange.x} - {weaponDamageRange.y}";
        else
            weaponDamageText.text = weaponDamageRange.x.ToString();
        
        // Damage types
        List<DamageType> damageTypes = weapon.GetDamageTypes();
        if (damageTypes.Count > 1)
        {
            weaponDamageTypeText.text = string.Join(", ", damageTypes);
        }
        else if (damageTypes.Count == 1)
        {
            weaponDamageTypeText.text = damageTypes[0].name;
        }
        
        panel.SetActive(true);
    }

    /// <summary>
    /// Hides the feedback.
    /// </summary>
    private void Hide()
    {
        panel.SetActive(false);
    }

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void WeaponSelectionButton_OnAnyWeaponSelectionButtonEnter(object sender, FM_WeaponSelectionButton enteredButton)
    {
        Display(enteredButton);
    }
    
    private void WeaponSelectionButton_OnAnyWeaponSelectionButtonExit(object sender, FM_WeaponSelectionButton exitedButton)
    {
        Hide();
    }
}