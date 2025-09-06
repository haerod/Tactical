using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class FM_WeaponSelectionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    private U__Unit character;
    private FM_WeaponButtonsHolder holder;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Sets the parameters to the button.
    /// </summary>
    /// <param name="linkedHolder"></param>
    /// <param name="linkedCharacter"></param>
    public void SetParameters(FM_WeaponButtonsHolder linkedHolder, U__Unit linkedCharacter)
    {
        holder = linkedHolder;
        character = linkedCharacter;
    }

    /// <summary>
    /// Displays the button with the weapon's infos.
    /// </summary>
    /// <param name="weapon"></param>
    public void DisplayButton(Weapon weapon)
    {
        weaponSprite.sprite = weapon.GetIcon();
        buttonText.text = weapon.GetName();
        button.onClick.AddListener(delegate
            {
                character.weaponHolder.SetCurrentWeapon(weapon);
                holder.CreateWeaponButtons(character);
            });
        
        if(weapon == character.weaponHolder.GetCurrentWeapon())
            button.interactable = false;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
