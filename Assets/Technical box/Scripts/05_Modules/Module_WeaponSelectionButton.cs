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
    
    public Weapon weapon { get; private set; }
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Displays the button with the weapon's infos.
    /// </summary>
    /// <param name="weaponToDisplay"></param>
    /// <param name="showText"></param>
    public void DisplayButton(Weapon weaponToDisplay, Action OnClick, bool showText)
    {
        weapon = weaponToDisplay;
        weaponSprite.sprite = weapon.icon;
        
        if(showText)
            buttonText.text = weapon.itemName;
        else
            buttonText.transform.parent.gameObject.SetActive(false);

        button.onClick.AddListener(delegate { OnClick();});
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
