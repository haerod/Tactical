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
    [Space]
    [SerializeField] private GameColor coveredColor;
    [SerializeField] private GameColor uncoveredColor;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        InputEvents.OnOccupiedTile += InputEvents_OnOccupiedTile;
        _input.OnTileExit += Input_OnTileExit;
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        c.health.OnDeath += Health_OnDeath;
        c.health.HealthChanged += Health_HealthChanged;
        DisplayCharacterCoverState(c.cover.GetCoverState());
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Orients the in-world UI to the camera.
    /// </summary>
    public void OrientToCamera() => orientToCamera.OrientToCamera();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the in-world UI over the character (health bar, cover state, etc.).
    /// </summary>
    private void Display()
    {
        healthBar.gameObject.SetActive(true);
        coverState.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Hides the in-world UI over the character (health bar, cover state, etc.).
    /// </summary>
    private void Hide()
    {
        healthBar.gameObject.SetActive(false);
        coverState.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Displays the cover state of the character on its world UI (hover it).
    /// </summary>
    /// <param name="coverInfo"></param>
    private void DisplayCharacterCoverState(CoverInfo coverInfo)
    {
        if (coverInfo == null)
            coverState.HideCoverState();
        else
            coverState.DisplayCoverState(
                coverInfo.GetCoverType(),
                coverInfo.GetIsCovered() ? coveredColor.color : uncoveredColor.color);
    }
    
    /// <summary>
    /// Updates the health bar values.
    /// </summary>
    private void UpdateHealthBar() => healthBar.DisplayCurrentHealth();
    
    /// <summary>
    /// Starts a waits for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits for "time" seconds and executes an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void InputEvents_OnOccupiedTile(object sender, Tile hoveredTile)
    {
        if(hoveredTile.character == c)
            Display();
    }
    
    private void Input_OnTileExit(object sender, Tile exitedTile)
    {
        if(exitedTile.character != c)
            return; // Not this character
        if(_characters.current == c)
            return; // Is the current character
        
        Hide();
    }
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if(startingCharacter == c)
            Display();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if(endingCharacter == c)
            Hide();
    }
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        Wait(1, () => { c.unitUI.Hide(); });
    }
    
    private void Health_HealthChanged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}
