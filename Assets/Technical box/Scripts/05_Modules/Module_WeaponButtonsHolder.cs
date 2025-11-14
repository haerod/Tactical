using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class Module_WeaponButtonsHolder : MonoBehaviour
{
    [SerializeField] private GameObject buttonSelectWeaponPrefab;
    [SerializeField] private Transform buttonsParent;
    
    private List<GameObject> instantiatedButtons = new List<GameObject>();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Destroys the old buttons, creates new ones and gives them parameters.
    /// </summary>
    /// <param name="unit"></param>
    public void CreateWeaponButtons(U__Unit unit)
    {
        DestroyAllButtons();
        instantiatedButtons.Clear();
        
        foreach (Weapon weapon in unit.weaponHolder.GetWeaponList())
        {
            Module_WeaponSelectionButton instantiateButton = 
                Instantiate(
                    buttonSelectWeaponPrefab,  
                    buttonsParent)
                    .GetComponent<Module_WeaponSelectionButton>();
            
            instantiateButton.SetParameters(this, unit);
            instantiateButton.DisplayButton(weapon);
            
            instantiatedButtons.Add(instantiateButton.gameObject);
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
        {
            Destroy(child.gameObject);
        }
    }

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.CanPlay())
            return; // Can't play
        if(!startingUnit.behavior.playable)
            return; // Not playable character
        
        CreateWeaponButtons(startingUnit);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        DestroyAllButtons();
    }
}
