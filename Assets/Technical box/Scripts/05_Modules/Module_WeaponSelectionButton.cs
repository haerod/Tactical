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
    
    private Unit unit;
    private Module_WeaponButtonsHolder holder;

    public Weapon weapon { get; private set; }
    
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
    public void SetParameters(Module_WeaponButtonsHolder linkedHolder, Unit linkedCharacter)
    {
        holder = linkedHolder;
        unit = linkedCharacter;
    }

    /// <summary>
    /// Displays the button with the weapon's infos.
    /// </summary>
    /// <param name="weaponToDisplay"></param>
    public void DisplayButton(Weapon weaponToDisplay, bool showText)
    {
        weapon = weaponToDisplay;
        weaponSprite.sprite = weapon.icon;
        
        if(showText)
            buttonText.text = weapon.itemName;
        else
            buttonText.transform.parent.gameObject.SetActive(false);
        button.onClick.AddListener(delegate
            {
                unit.weaponHolder.EquipWeapon(weapon);
                holder.CreateWeaponButtons(unit);
            });
        
        if(weapon == unit.weaponHolder.weapon)
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
