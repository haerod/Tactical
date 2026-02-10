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
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _buttonText;
    
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
    public void DisplayButton(Weapon weaponToDisplay, Action OnClick, bool showText, bool isCurrent)
    {
        weapon = weaponToDisplay;
        _icon.sprite = weapon.icon;
        
        if(showText)
            _buttonText.text = weapon.itemName;
        else
            _buttonText.transform.parent.gameObject.SetActive(false);
        
        if(isCurrent)
            _button.interactable = false;
        
        _button.onClick.AddListener(delegate { OnClick();});
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
