using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using static M__Managers;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class Module_ActionSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    private Unit unit;
    private Module_WeaponButtonsHolder holder;
    
    public A__Action action { get; private set; }
    
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
    /// <param name="actionToDisplay"></param>
    public void DisplayButton(A__Action actionToDisplay, Action OnClick, bool showText)
    {
        action = actionToDisplay;
        icon.sprite = actionToDisplay.icon;
        
        if(showText)
            buttonText.text = actionToDisplay.actionName;
        else
            buttonText.transform.parent.gameObject.SetActive(false);
        button.onClick.AddListener(delegate {OnClick();});
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameEvents.InvokeOnAnyWeaponSelectionButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //GameEvents.InvokeOnAnyWeaponSelectionButtonExit(this);
    }
}
