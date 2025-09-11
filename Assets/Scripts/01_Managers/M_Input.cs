﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Input : MonoBehaviour
{
    [Header("GAMEPLAY")]

    [SerializeField] private KeyCode recenterCameraKey = KeyCode.Space;    
    [SerializeField] private KeyCode changeUnitKey = KeyCode.Tab;
    [SerializeField] private KeyCode endTurnKey = KeyCode.Backspace;

    [Header("CAMERA")]

    [SerializeField] private KeyCode upKey = KeyCode.Z;
    [SerializeField] private KeyCode downKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.Q;
    [SerializeField] private KeyCode rightKey = KeyCode.D;
    [Space]
    [SerializeField] private KeyCode rotateRightKey = KeyCode.A;
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.E;
    [Space]
    [SerializeField] private KeyCode zoomInKey = KeyCode.T;
    [SerializeField] private KeyCode zoomOutKey = KeyCode.G;
    
    public event EventHandler<bool> OnChangeClickActivation;

    public event EventHandler<Coordinates> OnMovingCameraInput;
    public event EventHandler<int> OnZoomingCameraInput;
    public event EventHandler OnRecenterCameraInput;
    public event EventHandler OnEndTurnInput;
    public event EventHandler OnChangeUnitInput;
    public event EventHandler OnRotateLeftInput;
    public event EventHandler OnRotateRightInput;
    
    private bool canUsePlayerInput = true;
    private Tile previousTile;
    private U__Unit previousUnit;
    private Plane floorPlane = new (Vector3.up, Vector3.zero);
    public static M_Input instance;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Input in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _rules.OnVictory += Rules_OnVictory;
        A__Action.OnAnyActionStart += Action_OnAnyActionStart;
        A__Action.OnAnyActionEnd += Action_OnAnyActionEnd;
    }

    private void Update()
    {
        if (!canUsePlayerInput) 
            return; // Player can't click
        if (IsPointerOverUI()) 
            return; // Pointer over UI

        CheckRaycast();
        CheckClick();
        CheckChangeUnitInput();
        CheckEndTurnInput();
        CheckRecenterCameraInput();
        CheckCameraMovementInput();
        CheckCameraZoomInput();
        CheckCameraRotationInput();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Gets the pointer position on the floor (Plane at y = 0).
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPointerPosition()
    {
        //Create a ray from the Mouse click position
        Ray ray = _camera.GetCurrentCamera().ScreenPointToRay(Input.mousePosition);
        floorPlane.Raycast(ray, out float enter); 
        return ray.GetPoint(enter);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if pointer is over UI. Else, returns false.
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    
    /// <summary>
    /// Check over which object the pointer is (with raycast).
    /// </summary>
    private void CheckRaycast()
    {
        Ray ray = _camera
            .GetCurrentCamera()
            .ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile;
            U__Unit unit;

            // On a unit's collider, get the unit's tile
            if (hit.transform.CompareTag("Clickable"))
            {
                unit = hit.transform.GetComponentInParent<U__Unit>();
                tile = unit.tile;
            }
            else
            {
                tile = hit.transform.GetComponent<Tile>();
                unit = tile.character;
            }

            if (tile == previousTile)
                return; // Already on pointed tile / unit

            if(previousTile)
                InputEvents.TileUnhovered(previousTile);
            if(previousUnit)
                InputEvents.UnitUnhovered(previousUnit);
            
            previousTile = tile;
            previousUnit = unit;

            if (tile)
                InputEvents.TileHovered(tile);
            if(unit)
                InputEvents.UnitHovered(unit);
        }
        else
        {
            if (previousTile)
            {
                InputEvents.TileUnhovered(previousTile);
                previousTile = null;
            }

            if (previousUnit)
            {
                InputEvents.UnitUnhovered(previousUnit);
                previousUnit = null;
            }
            
            InputEvents.NothingHovered();
        }
    }
    
    /// <summary>
    /// Checks if player click on a tile (or element on a tile), previously checked by CheckRaycast().
    /// </summary>
    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0)) 
            return; // No click
        if (!previousTile) 
            return; // Not on a tile

        if (previousTile.character)
            InputEvents.UnitClick(previousTile.character);
        else
            InputEvents.TileClick(previousTile);
    }
    
    /// <summary>
    /// Checks input to change unit to another in the same team.
    /// </summary>
    private void CheckChangeUnitInput()
    {
        if (Input.GetKeyDown(changeUnitKey))
            OnChangeUnitInput?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Checks input to end turn and passes to the next team turn.
    /// </summary>
    private void CheckEndTurnInput()
    {
        if (Input.GetKeyDown(endTurnKey))
            OnEndTurnInput?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Checks input to recenter camera on the current character
    /// </summary>
    private void CheckRecenterCameraInput()
    {
        if (Input.GetKeyDown(recenterCameraKey))
            OnRecenterCameraInput?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Checks if mouse is on the borders (so haves to move) or movement keys are pressed.
    /// </summary>
    private void CheckCameraMovementInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        bool upInput = mousePosition.y >= Screen.height || Input.GetKey(upKey);
        bool downInput = mousePosition.y <= 0 || Input.GetKey(downKey);
        bool leftInput = mousePosition.x <= 0 || Input.GetKey(leftKey);
        bool rightInput = mousePosition.x >= Screen.width || Input.GetKey(rightKey);
        Coordinates direction = new(0,0);

        if (upInput)
            direction += new Coordinates(0,1);
        if (downInput)
            direction += new Coordinates(0,-1);
        if (leftInput)
            direction += new Coordinates(-1,0);
        if (rightInput)
            direction += new Coordinates(1,0);

        OnMovingCameraInput?.Invoke(this, direction);
    }
    
    /// <summary>
    /// Checks if zoom input is pressed.
    /// </summary>
    private void CheckCameraZoomInput()
    {
        int zoomAmount = 0;

        if (Input.GetKey(zoomInKey) || Input.mouseScrollDelta.y > 0)
            zoomAmount -= 1;
        if (Input.GetKey(zoomOutKey) || Input.mouseScrollDelta.y < 0)
            zoomAmount += 1;

        if (zoomAmount == 0)
            return; // No input

        OnZoomingCameraInput?.Invoke(this, zoomAmount);
    }
    
    private void CheckCameraRotationInput()
    {
        if(Input.GetKeyDown(rotateRightKey))
            OnRotateRightInput?.Invoke(this, EventArgs.Empty);
        if (Input.GetKeyDown(rotateLeftKey))
            OnRotateLeftInput?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Sets if players can use inputs or not.
    /// </summary>
    /// <param name="value"></param>
    private void SetActivePlayerInput(bool value = true)
    {
        canUsePlayerInput = value;
        OnChangeClickActivation?.Invoke(this, value);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit )
    {
        if (startingUnit.behavior.playable) 
            _input.SetActivePlayerInput();
        else
            _input.SetActivePlayerInput(false);

        previousTile = null;
        previousUnit = null;
    }
    
    private void Action_OnAnyActionStart(object sender, U__Unit unitInAction)
    {
        SetActivePlayerInput(false);
    }
    
    private void Action_OnAnyActionEnd(object sender, U__Unit unitInAction)
    {
        SetActivePlayerInput();
    }
    
    private void Rules_OnVictory(object sender, EventArgs e)
    {
        SetActivePlayerInput(false);
    }
}

public static class InputEvents
{
    public static event EventHandler<Tile> OnTileEnter;
    public static event EventHandler<Tile> OnFreeTileEnter;
    public static event EventHandler<Tile> OnTileExit;
    public static event EventHandler<Tile> OnTileClick;
    public static event EventHandler OnNoTile;
    
    public static event EventHandler<U__Unit> OnUnitEnter;
    public static event EventHandler<U__Unit> OnUnitExit;
    public static event EventHandler <U__Unit> OnUnitClick;
    public static event EventHandler<U__Unit> OnEnemyEnter;
    public static event EventHandler<U__Unit> OnAllyEnter;
    public static event EventHandler OnCurrentUnitEnter;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public static void TileHovered(Tile tile) => TileHoveredEvents(tile);
    public static void TileUnhovered(Tile tile) => OnTileExit?.Invoke(null, tile);
    public static void TileClick(Tile tile) => OnTileClick?.Invoke(null, tile);
    public static void UnitUnhovered(U__Unit unit) => OnUnitExit?.Invoke(null, unit);
    public static void UnitHovered(U__Unit unit) => CharacterHoveredEvents(unit);
    public static void UnitClick(U__Unit unit)  => OnUnitClick?.Invoke(null, unit);
    public static void NothingHovered() => OnNoTile?.Invoke(null, EventArgs.Empty);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Events happening if the pointer overlaps a tile.
    /// </summary>
    /// <param name="tile"></param>
    private static void TileHoveredEvents(Tile tile)
    {
        OnTileEnter?.Invoke(null, tile);
        
        U__Unit currentUnit = _units.current;
        
        if (!currentUnit.move.CanWalkAt(tile.coordinates) || !currentUnit.CanPlay()) 
            return; // Can't go on this tile or can't play
        
        bool pointedCharacterIsVisible = !_rules.IsFogOfWar() || currentUnit.look.visibleTiles.Contains(tile);

        if (tile.IsOccupiedByCharacter() && pointedCharacterIsVisible)
        {
            CharacterHoveredEvents(tile.character);
            return; // Tile occupied by a unit
        }
        
        OnFreeTileEnter?.Invoke(null, tile);
    }
    
    /// <summary>
    /// Events happening if the pointer overlaps a occupied by a unit.
    /// </summary>
    /// <param name="hoveredCharacter"></param>
    private static void CharacterHoveredEvents(U__Unit hoveredCharacter)
    {
        U__Unit currentUnit = _units.current;
        U__Unit currentTarget = hoveredCharacter;
        
        OnUnitEnter?.Invoke(null, currentTarget);
        
        if (currentUnit.team.IsAllyOf(currentTarget)) // Unit or allie
        {
            if (currentUnit == currentTarget)
                OnCurrentUnitEnter?.Invoke(null, EventArgs.Empty);
            else
                OnAllyEnter?.Invoke(null, currentTarget);
        }
        else // Enemy
            OnEnemyEnter?.Invoke(null, currentTarget);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}
