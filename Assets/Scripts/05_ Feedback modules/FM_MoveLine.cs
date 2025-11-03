using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;

public class FM_MoveLine : MonoBehaviour
{
    [Range(.01f, .5f)]
    [SerializeField] private float lineOffset = 0.05f;

    [Header("REFERENCES")]

    [SerializeField] private LineRenderer line;
    [SerializeField] private LineRenderer lineOut;

    private U__Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _rules.OnVictory += Rules_OnVictory;
    }

    private void OnDisable()
    {
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        _rules.OnVictory -= Rules_OnVictory;
        
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        
        if(!currentUnit)
            return;
        
        currentUnit.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
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
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        if(!startingUnit.CanPlay())
            return; // Can't play
        
        currentUnit = startingUnit;
        
        currentUnit.move.OnMovableTileEnter += Move_OnMovableTileEnter;
        currentUnit.move.OnMovementStart += Move_OnMovementStart;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;   
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        DisableLines();
        
        currentUnit.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        
        currentUnit = null;
    }
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void Move_OnMovableTileEnter(object sender, List<Tile> pathfinding)
    {
        SetLines(pathfinding, _units.current.move.GetMovementRange());
    }

    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        DisableLines();
    }
    
    private void InputEvents_OnUnitEnter(object sender, U__Unit hoveredCharacter)
    {
        if(!_units.current.look.CharactersVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
        
        DisableLines();
    }
}