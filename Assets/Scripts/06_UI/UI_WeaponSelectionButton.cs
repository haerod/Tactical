using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class UI_WeaponSelectionButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    private C__Character character;
    private UI_WeaponButtonsHolder holder;
    
    public static event EventHandler<Weapon> OnAnyWeaponChanged;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Sets the parameters to the button.
    /// </summary>
    /// <param name="linkedCharacter"></param>
    public void SetParameters(UI_WeaponButtonsHolder linkedHolder, C__Character linkedCharacter)
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
                character.weaponHolder.DisplayWeapon(weapon);
                holder.CreateWeaponButtons(character);
                OnAnyWeaponChanged?.Invoke(this, weapon);
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
