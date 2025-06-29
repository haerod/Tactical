using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class C_UnitUI : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private C__Character c;
    [SerializeField] private UI_SlicedHealthBar healthBar;
    [SerializeField] private UI_CoverState coverState;
    [SerializeField] private UI_OrientToCamera orientToCamera;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterHover += Characters_OnCharacterHover;
        _characters.OnCharacterExit += Characters_OnCharacterExit;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the in-world UI over the character (health bar, cover state, etc.).
    /// </summary>
    public void Display()
    {
        healthBar.gameObject.SetActive(true);
        coverState.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the in-world UI over the character (health bar, cover state, etc.).
    /// </summary>
    public void Hide()
    {
        healthBar.gameObject.SetActive(false);
        coverState.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the health bar values.
    /// </summary>
    public void UpdateHealthBar() => healthBar.DisplayCurrentHealth();

    /// <summary>
    /// Hides the cover state's info.
    /// </summary>
    public void HideCoverState() => coverState.HideCoverState();

    /// <summary>
    /// Display the cover state's info.
    /// </summary>
    /// <param name="coverType"></param>
    /// <param name="uncoveredColour"></param>
    public void DisplayCoverState(CoverType coverType, Color uncoveredColour) => coverState.DisplayCoverState(coverType, uncoveredColour);

    /// <summary>
    /// Orients the in-world UI to the camera.
    /// </summary>
    public void OrientToCamera() => orientToCamera.OrientToCamera();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Characters_OnCharacterHover(object sender, C__Character hoverCharacter)
    {
        if(hoverCharacter != c)
            return; // Not this character
        
        Display();
    }

    private void Characters_OnCharacterExit(object sender, C__Character exitCharacter)
    {
        if(exitCharacter != c)
            return; // Not this character
        if(_characters.current == c)
            return; // Is the current character
        
        Hide();
    }
}
