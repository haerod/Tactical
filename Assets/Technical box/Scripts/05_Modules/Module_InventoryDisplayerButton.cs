using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static M__Managers;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class Module_InventoryDisplayerButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    public Item item { get; private set; }
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Displays the button with the item's infos.
    /// </summary>
    /// <param name="itemToDisplay"></param>
    /// <param name="isLargeIcon"></param>
    public void DisplayButton(Item itemToDisplay, bool isLargeIcon = false)
    {
        if (itemToDisplay == null)
        {
            icon.gameObject.SetActive(false);
            buttonText.gameObject.SetActive(false);
            GetComponentInChildren<TooltipTrigger_Item>().enabled = false;
            GetComponentInChildren<Button>().interactable = false;
            GetComponentInChildren<Outline>().enabled = false;
            return; // No item
        }
        
        item = itemToDisplay;
        icon.sprite = item.icon;
        
        if(isLargeIcon)
        {
            icon.CrossFadeAlpha(.5f, 0, true);
            GetComponentInChildren<TooltipTrigger_Item>().enabled = false;
            GetComponentInChildren<Button>().interactable = false;
            GetComponentInChildren<Outline>().enabled = false;
        }
        
        buttonText.text = item.stackable ? item.currentStack.ToString() : "";
        buttonText.gameObject.SetActive(item.stackable);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
