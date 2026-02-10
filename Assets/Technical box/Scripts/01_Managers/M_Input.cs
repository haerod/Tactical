using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using static M__Managers;

public class M_Input : MonoBehaviour
{
    [Header("- GAMEPLAY -")][Space]

    [SerializeField] private KeyCode recenterCameraKey = KeyCode.Space;    
    [SerializeField] private KeyCode changeUnitKey = KeyCode.Tab;
    [SerializeField] private KeyCode endTurnKey = KeyCode.Backspace;
    [SerializeField] private KeyCode reloadWeaponKey = KeyCode.R;

    [Header("- CAMERA -")][Space]
    
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
    
    public event EventHandler<bool> OnClickActivationChanged;

    public event EventHandler<Coordinates> OnMovingCameraInput;
    public event EventHandler<int> OnZoomingCameraInput;
    public event EventHandler OnRecenterCameraInput;
    public event EventHandler OnEndTeamTurnInput;
    public event EventHandler OnNextTeammateInput;
    public event EventHandler OnReloadWeaponInput;
    public event EventHandler OnRotateLeftInput;
    public event EventHandler OnRotateRightInput;
    
    private bool canUsePlayerInput = true;
    private Tile previousTile;
    private Unit previousUnit;
    private Plane floorPlane = new (Vector3.up, Vector3.zero);
    
    public static M_Input instance => _instance == null ? FindFirstObjectByType<M_Input>() : _instance;
    public static M_Input _instance;
    
    public bool isPointerOverUI => EventSystem.current.IsPointerOverGameObject();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Awake()
    {
        // Singleton
        if (!_instance)
            _instance = this;
        else
            Debug.LogError("There is more than one M_Input in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }
    
    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _level.OnVictory += Level_OnVictory;
        GameEvents.OnAnyActionStart += Action_OnAnyActionStart;
        GameEvents.OnAnyActionEnd += Action_OnAnyActionEnd;
    }

    private void OnDisable()
    {
        OnClickActivationChanged = null;
        OnEndTeamTurnInput = null;
        OnNextTeammateInput = null;
        OnReloadWeaponInput = null;
        OnRotateLeftInput = null;
        OnRotateRightInput = null;
        OnZoomingCameraInput = null;
        OnRecenterCameraInput = null;
        
        InputEvents.ClearEvents();
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
        CheckReloadWeaponInput();
        CheckRecenterCameraInput();
        CheckCameraMovementInput();
        CheckCameraZoomInput();
        CheckCameraRotationInput();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
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
        if (isPointerOverUI)
        {
            InputEvents.PointerOverUI();
            return; // Pointer over UI
        }
        
        Ray ray = _camera
            .currentCamera
            .ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile;
            Unit unit;

            // On a unit's collider, get the unit's tile
            if (hit.transform.CompareTag("Clickable"))
            {
                unit = hit.transform.GetComponentInParent<Unit>();
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
            OnNextTeammateInput?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Checks input to end turn and passes to the next team turn.
    /// </summary>
    private void CheckEndTurnInput()
    {
        if (Input.GetKeyDown(endTurnKey))
            OnEndTeamTurnInput?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Checks input to reload current unit's weapon.
    /// </summary>
    private void CheckReloadWeaponInput()
    {
        if (Input.GetKeyDown(reloadWeaponKey))
            OnReloadWeaponInput?.Invoke(this, EventArgs.Empty);
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
        OnClickActivationChanged?.Invoke(this, value);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit )
    {
        if (startingUnit.behavior.playable) 
            _input.SetActivePlayerInput();
        else
            _input.SetActivePlayerInput(false);

        previousTile = null;
        previousUnit = null;
    }
    
    private void Action_OnAnyActionStart(object sender, Unit unitInAction)
    {
        SetActivePlayerInput(false);
    }
    
    private void Action_OnAnyActionEnd(object sender, Unit unitInAction)
    {
        SetActivePlayerInput();
    }
    
    private void Level_OnVictory(object sender, Team winnerTeam)
    {
        SetActivePlayerInput(false);
    }
}