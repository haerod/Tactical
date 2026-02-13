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
    [SerializeField] private Texture2D outOfAmmoCursor;
    
    private enum CursorType { Regular, AimAndInSight, OutAimOrSight, OutMovement, Heal, OutOfAmmo } // /!\ If add/remove a cursor, update the SetCursor method
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnAllyEnter += InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter += InputEvents_OnCurrentUnitEnter;
        InputEvents.OnNoTile += InputEvents_OnNoTile;
        InputEvents.OnPointerOverUI += InputEvents_OnPointerOverUI;
        _input.OnClickActivationChanged += Input_OnClickActivationChanged;
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
            case CursorType.OutOfAmmo:
                Cursor.SetCursor(outOfAmmoCursor, new Vector2(16, 16), CursorMode.Auto);
                break;
            default:
                break;
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
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
        Unit unit = _units.current;
        
        if(!unit.CanPlay())
            return; // Unit can't play
        
        if (!unit.look.CanSee(hoveredEnemy))
        {
            SetCursor(CursorType.OutAimOrSight);
            return; // Unit not in sight
        }
        if(!unit.attack.AttackableTiles().Contains(hoveredEnemy.tile))
            return; // Not attackable
        
        if (unit.weaponHolder.weaponData.usesAmmo && unit.weaponHolder.weapon.currentLoadedAmmo == 0)
        {
            SetCursor(CursorType.OutOfAmmo);
            return; // Out of ammo
        }
        
        SetCursor(CursorType.AimAndInSight);
    }
    
    private void InputEvents_OnCurrentUnitEnter(object sender, EventArgs e)
    {
        SetCursor(CursorType.Regular);
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile tile)
    {
        Unit current = _units.current;
        
        if(!current)
            return; // No current unit
        
        SetCursor(current.move.CanMoveTo(tile) && current.CanPlay() ? CursorType.Regular : CursorType.OutMovement);
    }
    
    private void InputEvents_OnTileExit(object sender, Tile tile)
    {
        SetCursor(CursorType.OutMovement);
    }
    
    private void Input_OnClickActivationChanged(object sender, bool canClickValue)
    {
        if(!canClickValue)
            SetCursor(CursorType.Regular);
    }
    
    private void InputEvents_OnPointerOverUI(object sender, EventArgs e)
    {
        SetCursor(CursorType.Regular);
    }
    
    private void InputEvents_OnNoTile(object sender, EventArgs e)
    {
        SetCursor(CursorType.OutMovement);
    }
}
