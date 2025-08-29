using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static M__Managers;

public class UI_Cursor : MonoBehaviour
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
        InputEvents.OnMovableTile += InputEvents_OnMovableTile;
        InputEvents.OnHoverAlly += InputEvents_OnHoverAlly;
        InputEvents.OnHoverEnemy += InputEvents_OnHoverEnemy;
        InputEvents.OnHoverItself += InputEvents_OnHoverItself;
        _input.OnTileExit += Input_OnTileExit;
        _input.OnChangeClickActivation += Input_ChangeClickActivation;
        _input.OnTileEnter += Input_OnTileEnter;
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
    
    private void InputEvents_OnMovableTile(object sender, List<Tile> pathfinding)
    {
        Tile endTile = pathfinding.LastOrDefault();
        
        if (!endTile)
        {
            SetCursor(CursorType.OutMovement);
            return; // No path
        }
        
        bool tileInMoveRange = _characters.current.move.CanMoveTo(endTile);

        // Set cursor
        SetCursor(tileInMoveRange ? CursorType.Regular : CursorType.OutMovement);
    }
    
    private void InputEvents_OnHoverAlly(object sender, C__Character hoveredAlly)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.look.HasSightOn(hoveredAlly.tile))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Character not in sight
        }
        if(!currentCharacter.actions.HasHealAction())
            return; // Character can't heal
        if (hoveredAlly.health.IsFullLife())
            return; // Target is full life
            
        SetCursor(CursorType.Heal);
    }
    
    private void InputEvents_OnHoverEnemy(object sender, C__Character hoveredEnemy)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.look.HasSightOn(hoveredEnemy.tile))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Character not in sight
        }
        if(!currentCharacter.attack.AttackableTiles().Contains(hoveredEnemy.tile))
            return; // Not attackable

        SetCursor(CursorType.AimAndInSight);
    }
    
    private void InputEvents_OnHoverItself(object sender, EventArgs e)
    {
        SetCursor(CursorType.Regular);
    }
    
    private void Input_OnTileExit(object sender, Tile tile)
    {
        SetCursor(CursorType.OutMovement);
    }

    private void Input_ChangeClickActivation(object sender, bool canClickValue)
    {
        if(!canClickValue)
            SetCursor(CursorType.Regular);
    }
    
    private void Input_OnTileEnter(object sender, Tile tile)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.move.CanWalkAt(tile.coordinates) || !currentCharacter.CanPlay()) 
            SetCursor(CursorType.OutMovement);
    }
}
