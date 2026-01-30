using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static M__Managers;

public class Module_InventoryDisplayer : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject buttonItem;
    [SerializeField] private GameObject buttonShowHide;
    [SerializeField] private Image showHideImage;
    [SerializeField] private Transform buttonsParent;
        
    bool isShowing = false;
    Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        DestroyAllButtons();
        SetShowingIcon();
    }

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += GameEvents_OnAnyActionEnd;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ClickOnShowHideButton()
    {
        if(isShowing)
            DestroyAllButtons();
        else
            CreateItemButtons(currentUnit.inventory);
        
        isShowing = !isShowing;
        SetShowingIcon();
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Displays the show/hide button.
    /// </summary>
    private void Show()
    {
        if(!currentUnit)
            return; // No current unit
        
        buttonShowHide.SetActive(true);
        
        if(isShowing)
            CreateItemButtons(currentUnit.inventory);
    }
    
    /// <summary>
    /// Hides the show/hide button.
    /// </summary>
    private void Hide()
    {
        buttonShowHide.SetActive(false);
        DestroyAllButtons();
    }
    
    /// <summary>
    /// Turns the icon, depending on isShowing.
    /// </summary>
    private void SetShowingIcon() => showHideImage.transform.rotation =
        isShowing ? Quaternion.identity : Quaternion.Euler(180f, 0f, 0f);
    
    /// <summary>
    /// Destroys all the buttons.
    /// </summary>
    private void DestroyAllButtons()
    {
        foreach (Transform child in buttonsParent)
        {
            if(child.gameObject == buttonShowHide)
                continue; // Show hide button
            
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// Destroys the old buttons, creates new ones and gives them parameters.
    /// </summary>
    /// <param name="inventory"></param>
    private void CreateItemButtons(Inventory inventory)
    {
        DestroyAllButtons();
        
        foreach (Item item in inventory.items)
        {
            for (int i = 0; i < item.sizeInInventory; i++)
            {
                Module_InventoryDisplayerButton instantiateButton = 
                    Instantiate(
                            buttonItem,  
                            buttonsParent)
                        .GetComponent<Module_InventoryDisplayerButton>();
                
                instantiateButton.DisplayButton(item, i != 0);
            }
        }

        if (!inventory.hasMaxSize) 
            return; // No max size
        
        // Empty icons
        for (var i = 0; i < inventory.remainingSpace; i++)
        {
            Module_InventoryDisplayerButton instantiateButton =
                Instantiate(
                        buttonItem,
                        buttonsParent)
                    .GetComponent<Module_InventoryDisplayerButton>();

            instantiateButton.DisplayButton(null);
        }

    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.CanPlay())
            return; // Can't play
        if(!startingUnit.behavior.playable)
            return; // Not playable character

        currentUnit = startingUnit;
        Show();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        Hide();
        currentUnit = null;
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit actingUnit)
    {
        Hide();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if(!endingActionUnit.behavior.playable)
            return; // Not playable character
        
        Show();
    }
}
