using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class Module_WeaponButtonsHolder : MonoBehaviour
{
    [SerializeField] private bool _showText = true;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject buttonSelectWeaponPrefab;
    [SerializeField] private GameObject buttonSelectActionPrefab;
    [SerializeField] private Transform buttonsParent;
        
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        DestroyAllButtons();
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

    /// <summary>
    /// Destroys the old buttons, creates new ones and gives them parameters.
    /// </summary>
    /// <param name="unit"></param>
    public void CreateWeaponButtons(Unit unit)
    {
        DestroyAllButtons();
        
        Weapon currentWeapon = unit.weaponHolder.weapon;
        
        if(unit.actionsHolder.HasAvailableAction<A_TakeWeapon>())
        {
            foreach (Weapon weapon in unit.inventory.weapons)
            {
                if(weapon == currentWeapon)
                    continue; // Current weapon
            
                Module_WeaponSelectionButton instantiateButton = 
                    Instantiate(
                        buttonSelectWeaponPrefab,  
                        buttonsParent)
                        .GetComponent<Module_WeaponSelectionButton>();
            
                instantiateButton.SetParameters(unit);
                instantiateButton.DisplayButton(weapon, _showText);
            }
        }

        if (unit.actionsHolder.HasAvailableAction<A_Reload>())
        {
            Module_ActionSelectionButton instantiatedButton = Instantiate(
                buttonSelectActionPrefab,
                buttonsParent)
                .GetComponent<Module_ActionSelectionButton>();
            
            instantiatedButton.SetParameters(this,unit);
            instantiatedButton.DisplayButton(
                unit.actionsHolder.GetActionOfType<A_Reload>(),
                delegate { unit.actionsHolder.GetActionOfType<A_Reload>().StartReload(); },
                _showText);
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Destroys all the buttons.
    /// </summary>
    private void DestroyAllButtons()
    {
        foreach (Transform child in buttonsParent)
            Destroy(child.gameObject);
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
        
        CreateWeaponButtons(startingUnit);
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        DestroyAllButtons();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit actingUnit)
    {
        DestroyAllButtons();
    }
    
    private void GameEvents_OnAnyActionEnd(object sender, Unit endingActionUnit)
    {
        if(!endingActionUnit.CanPlay())
            return; // Can't play
        if(!endingActionUnit.behavior.playable)
            return; // Not playable character
        
        CreateWeaponButtons(endingActionUnit);
    }
}
