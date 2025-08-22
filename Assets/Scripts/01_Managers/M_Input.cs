using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static M__Managers;

public class M_Input : MonoBehaviour
{
    [Header("GAMEPLAY")]

    [SerializeField] private KeyCode recenterCameraKey = KeyCode.Space;    
    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;
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
    
    public event EventHandler<Tile> OnTileEnter;
    public event EventHandler<Tile> OnTileExit;
    public event EventHandler<Tile> OnTileClick;
    public event EventHandler OnNoTile;
    
    public event EventHandler<C__Character> OnCharacterEnter;
    public event EventHandler<C__Character> OnCharacterExit;
    public event EventHandler <C__Character> OnCharacterClick;

    public event EventHandler<Coordinates> OnMovingCameraInput;
    public event EventHandler<int> OnZoomingCameraInput;
    public event EventHandler OnRecenterCameraInput;
    public event EventHandler OnEndTurnInput;
    public event EventHandler OnChangeCharacterInput;
    public event EventHandler OnRotateLeftInput;
    public event EventHandler OnRotateRightInput;
    
    private bool canClick = true;
    private Tile previousTile;
    private C__Character previousCharacter;
    private Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
    public static M_Input instance;
    
    private C__Character currentCharacter => _characters.current;

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

    private void Update()
    {
        if (!canClick) 
            return; // Player can't click
        if (_ui.IsPointerOverUI()) 
            return; // Pointer over UI

        CheckRaycast();
        CheckClick();
        CheckChangeCharacterInput();
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
    /// Sets if players can click or not on board objects
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveClick(bool value = true)
    {
        canClick = value;
        OnChangeClickActivation?.Invoke(this, value);
    }

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
            C__Character character;

            // On a character's collider, get the character's tile
            if (hit.transform.CompareTag("Clickable"))
            {
                character = hit.transform.GetComponentInParent<C__Character>();
                tile = character.tile;
            }
            else
            {
                tile = hit.transform.GetComponent<Tile>();
                character = tile.character;
            }

            if (tile == previousTile)
                return; // Already on pointed tile / character

            if(previousTile)
                OnTileExit?.Invoke(this, previousTile);
            if(previousCharacter)
                OnCharacterExit?.Invoke(this, previousCharacter);
            
            previousTile = tile;
            previousCharacter = character;

            if(tile)
                OnTileEnter?.Invoke(this, previousTile);
            if(character)
                OnCharacterEnter?.Invoke(this, previousCharacter);
        }
        else
        {
            if (previousTile)
            {
                OnTileExit?.Invoke(this, previousTile);
                previousTile = null;
            }

            if (previousCharacter)
            {
                OnCharacterExit?.Invoke(this, previousCharacter);
                previousCharacter = null;
            }
            
            OnNoTile?.Invoke(this, EventArgs.Empty);
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
            OnCharacterClick?.Invoke(this, previousTile.character);
        else
            OnTileClick?.Invoke(this, previousTile);;
    }

    /// <summary>
    /// Checks input to change character to another in the same team.
    /// </summary>
    private void CheckChangeCharacterInput()
    {
        if (Input.GetKeyDown(changeCharacterKey))
            OnChangeCharacterInput?.Invoke(this, EventArgs.Empty);
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
        Coordinates direction = new Coordinates(0,0);

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
}
