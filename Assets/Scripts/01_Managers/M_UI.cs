using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoBehaviour
{
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
