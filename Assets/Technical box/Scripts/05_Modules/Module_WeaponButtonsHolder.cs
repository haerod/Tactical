using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class Module_WeaponButtonsHolder : MonoBehaviour
{
    [SerializeField] private bool _showText = true;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject buttonSelectWeaponPrefab;
    [SerializeField] private Transform buttonsParent;
    
    private List<GameObject> instantiatedButtons = new();

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
        instantiatedButtons.Clear();
        
        foreach (Weapon weapon in unit.inventory.weapons)
        {
            Module_WeaponSelectionButton instantiateButton = 
                Instantiate(
                    buttonSelectWeaponPrefab,  
                    buttonsParent)
                    .GetComponent<Module_WeaponSelectionButton>();
            
            instantiateButton.SetParameters(this, unit);
            instantiateButton.DisplayButton(weapon, _showText);
            
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
