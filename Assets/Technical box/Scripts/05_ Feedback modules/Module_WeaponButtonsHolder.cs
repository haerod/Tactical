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
    /// <param name="character"></param>
    public void CreateWeaponButtons(U__Unit character)
    {
        DestroyAllButtons();
        
        if(!character.behavior.playable)
            return; // Not playable character
        
        foreach (Weapon weapon in character.weaponHolder.GetWeaponList())
        {
            Module_WeaponSelectionButton instantiateButton = 
                Instantiate(
                    buttonSelectWeaponPrefab,  
                    buttonsParent)
                    .GetComponent<Module_WeaponSelectionButton>();
            
            instantiateButton.SetParameters(this, character);
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
        instantiatedButtons.ForEach(b => Destroy(b.gameObject));
        instantiatedButtons.Clear();
    }

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit character)
    {
        if(!character.CanPlay())
            return; // Can't play
            
        CreateWeaponButtons(character);
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit e)
    {
        DestroyAllButtons();
    }
}
