using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class F_SelectionSquare : MonoBehaviour
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
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        A_Attack.OnAnyAttackStart += Attack_OnAnyAttackStart;
        Turns.OnVictory += Turns_OnVictory;
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;
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

    private void Move_OnMovableTileEnter(object sender, List<Tile> pathfinding)
    {
        Tile lastTile = pathfinding.Last();

        bool tileInMoveRange = _characters.current.move.CanMoveTo(lastTile);

        SetSquareAt(lastTile.worldPosition, tileInMoveRange);
    }

    private void Attack_OnAnyAttackStart(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingTurnCharacter)
    {
        DisableSquare();
        
        if(endingTurnCharacter.behavior.playable)
            endingTurnCharacter.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
    }
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character hoveredCharacter)
    {
        DisableSquare();
    }
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if(startingCharacter.behavior.playable)
            startingCharacter.move.OnMovableTileEnter += Move_OnMovableTileEnter;
    }

}