using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class Module_SelectionSquare : MonoBehaviour
{
    [SerializeField] private Transform squareTransform;
    [Range(.01f, .5f)] [SerializeField] private float squareOffset = .01f;
    [SerializeField] private Color inRangeColor = Color.white;
    [SerializeField] private Color outRangeColor = Color.grey;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GameEvents.OnAnyActionStart += GameEvents_OnAnyActionStart;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        _level.OnVictory += Level_OnVictory;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Enables the selection square on the given position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="inRange"></param>
    private void SetSquareAt(Vector3 position, bool inRange)
    {
        squareTransform.gameObject.SetActive(true);
        squareTransform.position = position + Vector3.up * squareOffset;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = inRange ? inRangeColor : outRangeColor;
    }
    
    /// <summary>
    /// Disables the selection square.
    /// </summary>
    private void Hide()
    {
        squareTransform.gameObject.SetActive(false);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        if(!_units.current)
            return; // No current unit
        if(!_units.current.behavior.playable)
            return; // NPC
        
        SetSquareAt(enteredTile.worldPosition, _units.current.move.CanMoveTo(enteredTile));
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredCharacter)
    {
        if(!_units.current.look.UnitsVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
        
        Hide();
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        Hide();
    }
    
    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        Hide();
    }
    
    private void GameEvents_OnAnyActionStart(object sender, Unit e)
    {
        Hide();
    }

    private void Level_OnVictory(object sender, EventArgs e)
    {
        Hide();
    }
}