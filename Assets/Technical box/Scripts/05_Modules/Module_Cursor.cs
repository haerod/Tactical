using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class Module_Cursor : MonoBehaviour
{
    [SerializeField] private Texture2D aimCursor;
    [SerializeField] private Texture2D outSightCursor;
    [SerializeField] private Texture2D cantGoCursor;
    [SerializeField] private Texture2D healCursor;
    
    private enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement, Heal } // /!\ If add/remove a cursor, update the SetCursor method
    
    private Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnAllyEnter += InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter += InputEvents_OnCurrentUnitEnter;
        
        _input.OnChangeClickActivation += Input_ChangeClickActivation;
    }

    private void OnDisable()
    {
        if(currentUnit)
            currentUnit.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Sets cursor to its new appearance.
    /// </summary>
    /// <param name="type"></param>
    private void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Regular:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.AimAndInSight:
                Cursor.SetCursor(aimCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutAimOrSight:
                Cursor.SetCursor(outSightCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.OutMovement:
                Cursor.SetCursor(cantGoCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            case CursorType.Heal:
                Cursor.SetCursor(healCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            default:
                break;
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Move_OnMovableTileEnter(object sender, List<Tile> pathfinding)
    {
        Tile endTile = pathfinding.LastOrDefault();
        
        if (!endTile)
        {
            SetCursor(CursorType.OutMovement);
            return; // No path
        }
        
        bool tileInMoveRange = _units.current.move.CanMoveTo(endTile);

        // Set cursor
        SetCursor(tileInMoveRange ? CursorType.Regular : CursorType.OutMovement);
    }
    
    private void InputEvents_OnAllyEnter(object sender, Unit hoveredAlly)
    {
        Unit currentCharacter = _units.current;
        
        if (!currentCharacter.look.CanSee(hoveredAlly))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Character not in sight
        }
        if(!currentCharacter.actionsHolder.HasAvailableAction<A_Heal>())
            return; // Character can't heal
        if(!currentCharacter.actionsHolder.GetActionOfType<A_Heal>().IsHealable(hoveredAlly))
            return; // Ally can't be healed (too far or full life)
        
        SetCursor(CursorType.Heal);
    }
    
    private void InputEvents_OnEnemyEnter(object sender, Unit hoveredEnemy)
    {
        Unit currentCharacter = _units.current;
        
        if(!currentCharacter.CanPlay())
            return; // Unit can't play
        
        if (!currentCharacter.look.CanSee(hoveredEnemy))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Unit not in sight
        }
        if(!currentCharacter.attack.AttackableTiles().Contains(hoveredEnemy.tile))
            return; // Not attackable

        SetCursor(CursorType.AimAndInSight);
    }
    
    private void InputEvents_OnCurrentUnitEnter(object sender, EventArgs e)
    {
        SetCursor(CursorType.Regular);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile tile)
    {
        SetCursor(CursorType.OutMovement);
    }

    private void Input_ChangeClickActivation(object sender, bool canClickValue)
    {
        if(!canClickValue)
            SetCursor(CursorType.Regular);
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile tile)
    {
        Unit current = _units.current;
        if(!current)
            return; // No current unit
        
        if (!current.move.CanWalkAt(tile.coordinates) || !current.CanPlay()) 
            SetCursor(CursorType.OutMovement);
    }
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC
        
        currentUnit = startingUnit;
        
        currentUnit.move.OnMovableTileEnter += Move_OnMovableTileEnter;
    }
    
    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        currentUnit.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
        
        currentUnit = null;
    }
}
