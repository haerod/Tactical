using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UIElements;
using static M__Managers;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class Module_WeaponSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    private U__Unit unit;
    private Module_WeaponButtonsHolder holder;
    private Weapon _weapon;
    public Weapon weapon;
    
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
    public void SetParameters(Module_WeaponButtonsHolder linkedHolder, U__Unit linkedCharacter)
    {
        holder = linkedHolder;
        unit = linkedCharacter;
    }

    /// <summary>
    /// Displays the button with the weapon's infos.
    /// </summary>
    /// <param name="weaponToDisplay"></param>
    public void DisplayButton(Weapon weaponToDisplay)
    {
        _weapon = weaponToDisplay;
        weaponSprite.sprite = _weapon.icon;
        buttonText.text = _weapon.itemName;
        button.onClick.AddListener(delegate
            {
                unit.weaponHolder.EquipWeapon(_weapon);
                holder.CreateWeaponButtons(unit);
            });
        
        if(_weapon == unit.weaponHolder.weapon)
            button.interactable = false;
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyWeaponSelectionButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameEvents.InvokeOnAnyWeaponSelectionButtonExit(this);
    }
}
