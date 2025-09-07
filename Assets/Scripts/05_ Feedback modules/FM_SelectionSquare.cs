using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class FM_SelectionSquare : MonoBehaviour
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
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
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
    private void DisableSquare()
    {
        squareTransform.gameObject.SetActive(false);
    }

    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        if(!startingCharacter.behavior.playable)
            return; // NPC
        
        startingCharacter.move.OnMovableTileEnter += Move_OnMovableTileEnter;
        startingCharacter.move.OnMovementStart += Move_OnMovementStart;
        startingCharacter.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingCharacter)
    {
        DisableSquare();
        
        if(!endingCharacter.behavior.playable)
            return; // NPC
        
        endingCharacter.move.OnMovableTileEnter -= Move_OnMovableTileEnter;       
        endingCharacter.move.OnMovementStart -= Move_OnMovementStart;
        endingCharacter.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
    }

    private void Move_OnMovableTileEnter(object sender, List<Tile> pathfinding)
    {
        Tile lastTile = pathfinding.Last();

        bool tileInMoveRange = _units.current.move.CanMoveTo(lastTile);

        SetSquareAt(lastTile.worldPosition, tileInMoveRange);
    }

    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void InputEvents_OnUnitEnter(object sender, U__Unit hoveredCharacter)
    {
        if(!_units.current.look.CharactersVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
        
        DisableSquare();
    }
}