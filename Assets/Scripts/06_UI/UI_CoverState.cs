using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class UI_CoverState : MonoBehaviour
{
    [SerializeField] private Image coverStateImage;
    [SerializeField] private C__Character unit;
    [SerializeField] private GameColor coveredColor;
    [SerializeField] private GameColor uncoveredColor;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        unit.health.OnDeath += Health_OnDeath;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Displays the cover state of the unit.
    /// </summary>
    private void Show()
    {
        CoverInfo coverInfo = unit.cover.GetCoverState();

        if (coverInfo == null)
        {
            Hide();
            return; // No cover info.
        }
        
        SetupCoverState(
            coverInfo.GetCoverType(),
            coverInfo.GetIsCovered() ? coveredColor.color : uncoveredColor.color);
        
        coverStateImage.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Shows the given information on the image.
    /// </summary>
    /// <param name="coverType"></param>
    /// <param name="color"></param>
    private void SetupCoverState(CoverType coverType, Color color)
    {
        Color colorWithAlpha = new Color(color.r, color.g, color.b, 1);
        coverStateImage.color = colorWithAlpha;
        coverStateImage.sprite = coverType.GetCoverFeedbackSprite();
    }
    
    /// <summary>
    /// Hides the cover state of the unit.
    /// </summary>
    private void Hide() => coverStateImage.gameObject.SetActive(false);
    
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
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingUnit)
    {
        if(startingUnit != unit)
            return; // Another unit's turn

        Show();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingUnit)
    {
        if(endingUnit != unit)
            return; // Another unit's turn
        
        Hide();
    }
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character hoveredUnit)
    {
        if(hoveredUnit != unit)
            return; // Another character
        
        C__Character currentUnit = _characters.current;
        
        if(currentUnit == unit)
            return; // Current character
        if(!currentUnit.look.CharactersVisibleInFog().Contains(unit))
            return; // Invisible character
        
        Show();
    }
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        Wait(1, Hide);
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        C__Character currentUnit = _characters.current;
        
        if(currentUnit.team.IsTeammateOf(unit))
            return; // Teammate
        if(!currentUnit.look.CanSee(unit))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(unit.look.visibleTiles.Contains(enteredTile))
            Hide();
        else
            Show();
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        if(exitedTile.character != unit)
            return; // Not this character
        if(_characters.current == unit)
            return; // Is the current character
        
        Hide();
    }
}
