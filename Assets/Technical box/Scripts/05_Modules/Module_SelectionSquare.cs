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

    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        _units.OnTeamTurnEnd += Units_OnTeamTurnEnd;
        _level.OnVictory += Level_OnVictory;
    }
    
    private void OnDisable()
    {
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

    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        currentUnit = startingUnit;
        
        currentUnit.move.OnMovableTileEnter += Move_OnMovableTileEnter;
        currentUnit.move.OnMovementStart += Move_OnMovementStart;
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        DisableSquare();
        
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        currentUnit.move.OnMovableTileEnter -= Move_OnMovableTileEnter;       
        currentUnit.move.OnMovementStart -= Move_OnMovementStart;
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        currentUnit = null;
    }

    private void Units_OnTeamTurnEnd(object sender, Team endingTeam)
    {
        DisableSquare();
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
    
    private void Level_OnVictory(object sender, EventArgs e)
    {
        DisableSquare();
    }
    
    private void InputEvents_OnUnitEnter(object sender, Unit hoveredCharacter)
    {
        if(!_units.current.look.UnitsVisibleInFog().Contains(hoveredCharacter))
            return; // Invisible character
        
        DisableSquare();
    }
}