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

public class FM_WeaponSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    private U__Unit unit;
    private FM_WeaponButtonsHolder holder;
    private Weapon weapon;
    
    public static event EventHandler<FM_WeaponSelectionButton> OnAnyWeaponSelectionButtonEnter;
    public static event EventHandler<FM_WeaponSelectionButton> OnAnyWeaponSelectionButtonExit;
    
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
        unit = linkedCharacter;
    }

    /// <summary>
    /// Displays the button with the weapon's infos.
    /// </summary>
    /// <param name="weaponToDisplay"></param>
    public void DisplayButton(Weapon weaponToDisplay)
    {
        weapon = weaponToDisplay;
        weaponSprite.sprite = weapon.GetIcon();
        buttonText.text = weapon.GetName();
        button.onClick.AddListener(delegate
            {
                unit.weaponHolder.SetCurrentWeapon(weapon);
                holder.CreateWeaponButtons(unit);
            });
        
        if(weapon == unit.weaponHolder.GetCurrentWeapon())
            button.interactable = false;
    }
    
    /// <summary>
    /// Returns the displayed weapon.
    /// </summary>
    /// <returns></returns>
    public Weapon GetWeapon() => weapon;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnAnyWeaponSelectionButtonEnter?.Invoke(this, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnAnyWeaponSelectionButtonExit?.Invoke(this, this);
    }
}
