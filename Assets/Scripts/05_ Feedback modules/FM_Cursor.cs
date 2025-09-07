using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class FM_Cursor : MonoBehaviour
{
    [SerializeField] private Texture2D aimCursor;
    [SerializeField] private Texture2D outSightCursor;
    [SerializeField] private Texture2D cantGoCursor;
    [SerializeField] private Texture2D healCursor;
    
    private enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement, Heal } // /!\ If add/remove a cursor, update the SetCursor method
    
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
    
    private void InputEvents_OnAllyEnter(object sender, U__Unit hoveredAlly)
    {
        U__Unit currentCharacter = _units.current;
        
        if (!currentCharacter.look.CanSee(hoveredAlly))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Character not in sight
        }
        if(!currentCharacter.actions.HasHealAction())
            return; // Character can't heal
        if(!currentCharacter.actions.GetHealAction().IsHealable(hoveredAlly))
            return; // Ally can't be healed (too far or full life)
            
        SetCursor(CursorType.Heal);
    }
    
    private void InputEvents_OnEnemyEnter(object sender, U__Unit hoveredEnemy)
    {
        U__Unit currentCharacter = _units.current;
        
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
        U__Unit currentCharacter = _units.current;
        
        if (!currentCharacter.move.CanWalkAt(tile.coordinates) || !currentCharacter.CanPlay()) 
            SetCursor(CursorType.OutMovement);
    }
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        if(startingCharacter.behavior.playable)
            startingCharacter.move.OnMovableTileEnter += Move_OnMovableTileEnter;
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingTurnCharacter)
    {
        if(endingTurnCharacter.behavior.playable)
            endingTurnCharacter.move.OnMovableTileEnter -= Move_OnMovableTileEnter;
    }
}
