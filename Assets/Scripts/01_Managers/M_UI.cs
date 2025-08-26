using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private Button nextTurnButton;

    public static M_UI instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_UI in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }

    private void Start()
    {
        _characters.GetCharacterList()
            .ForEach(character => DisplayCharacterCoverState(character, character.cover.GetCoverState()));
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Enables / disables player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Turn(bool value) => nextTurnButton.gameObject.SetActive(value);
    
    /// <summary>
    /// Enables / disables player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Action(bool value)
    {
        if (value == true && !_characters.current.behavior.playable) 
            return; // EXIT : it's not player's turn

        nextTurnButton.gameObject.SetActive(value);
    }
    
    /// <summary>
    /// Returns true if pointer is over UI. Else, returns false.
    /// </summary>
    /// <returns></returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays the cover state of the character on its world UI (hover it).
    /// </summary>
    /// <param name="character"></param>
    /// <param name="coverInfo"></param>
    private void DisplayCharacterCoverState(C__Character character, CoverInfo coverInfo)
    {
        if(coverInfo == null)
            character.unitUI.HideCoverState();
        else
            character.unitUI.DisplayCoverState(
                coverInfo.GetCoverType(), 
                coverInfo.GetIsCovered() ? _feedback.GetCoveredColour() : _feedback.GetUncoveredColour());
    }

    // ======================================================================
    // EVENTS
    // ======================================================================
}
