using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class F_MoveLine : MonoBehaviour
{
    [Range(.01f, .5f)]
    [SerializeField] private float lineOffset = 0.05f;

    [Header("REFERENCES")]

    [SerializeField] private LineRenderer line;
    [SerializeField] private LineRenderer lineOut;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Sets the lines on the path, with the good colors.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="movementRange"></param>
    private void SetLines(List<Tile> path, int movementRange)
    {
        bool isEndTileInMovementRange = path.Count - 1 <= movementRange;

        //Set line in and out active
        line.gameObject.SetActive(true);
        lineOut.gameObject.SetActive(!isEndTileInMovementRange);

        if (isEndTileInMovementRange)
            line.positionCount = path.Count;
        else
        {
            line.positionCount = movementRange + 1;
            lineOut.positionCount = path.Count - movementRange;
        }

        // Position line's points
        int i = 0;
        path.ForEach(tile => {
            if (i <= movementRange)
                line.SetPosition(i, tile.transform.position + Vector3.up * lineOffset);
            else
            {
                if (i == movementRange + 1)
                {
                    lineOut.SetPosition(i - (movementRange + 1), path[i - 1].transform.position + Vector3.up * lineOffset);
                }
                lineOut.SetPosition(i - (movementRange), tile.transform.position + Vector3.up * lineOffset);
            }

            i++;
        });
    }

    /// <summary>
    /// Disables the lines.
    /// </summary>
    private void DisableLines()
    {
        line.gameObject.SetActive(false);
        lineOut.gameObject.SetActive(false);
    }

    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if(!startingCharacter.behavior.playable)
            return; // NPC
        if(!startingCharacter.CanPlay())
            return; // Can't play
        
        startingCharacter.move.OnMovableTileEnter += Move_OnMovableTileEnter;
        startingCharacter.move.OnMovementStart += Move_OnMovementStart;
        startingCharacter.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;   
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if(!endingCharacter.behavior.playable)
            return; // NPC
        
        DisableLines();
        
        endingCharacter.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
        endingCharacter.move.OnMovementStart -= Move_OnMovementStart;
        endingCharacter.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnCharacterEnter -= InputEvents_OnCharacterEnter;   
    }
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void Move_OnMovableTileEnter(object sender, List<Tile> pathfinding)
    {
        SetLines(pathfinding, _characters.current.move.movementRange);
    }

    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character hoveredCharacter)
    {
        if(!_characters.current.look.CharactersVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
        
        DisableLines();
    }
    

}
