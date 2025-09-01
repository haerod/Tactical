using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class UI_WeaponButtonsHolder : MonoBehaviour
{
    [SerializeField] private GameObject buttonSelectWeaponPrefab;
    [SerializeField] private Transform buttonsParent;
    
    private List<GameObject> instantiatedButtons = new List<GameObject>();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Destroys the old buttons, creates new ones and gives them parameters.
    /// </summary>
    /// <param name="character"></param>
    public void CreateWeaponButtons(C__Character character)
    {
        DestroyAllButtons();
        
        if(!character.behavior.playable)
            return; // Not playable character
        
        foreach (Weapon weapon in character.weaponHolder.GetWeaponList())
        {
            UI_WeaponSelectionButton instantiateButton = 
                Instantiate(
                    buttonSelectWeaponPrefab,  
                    buttonsParent)
                    .GetComponent<UI_WeaponSelectionButton>();
            
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
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character character)
    {
        if(!character.CanPlay())
            return; // Can't play
            
        CreateWeaponButtons(character);
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character e)
    {
        DestroyAllButtons();
    }
}
