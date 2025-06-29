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
    [SerializeField] private KeyCode zoomInKey = KeyCode.T;
    [SerializeField] private KeyCode zoomOutKey = KeyCode.G;
    
    public event EventHandler<bool> OnChangeClickActivation;
    public event EventHandler<Tile> OnEnterTile;
    public event EventHandler<Tile> OnExitTile;
    
    public event EventHandler<Coordinates> OnMovingCameraInput;
    public event EventHandler<int> OnZoomingCameraInput;
    public event EventHandler OnRecenterCameraInput;
    public event EventHandler OnEndTurnInput;
    public event EventHandler OnChangeCharacterInput;
    
    public event EventHandler <C__Character> OnClickOnCharacter;
    public event EventHandler<Tile> OnClickOnTile;
    
    private bool canClick = true;
    private Tile pointedTile;
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
            Tile tile = hit.transform.GetComponent<Tile>();

            // On a character's collider, get the character's tile
            if (hit.transform.CompareTag("Clickable"))
                tile = hit.transform.GetComponentInParent<C__Character>().tile;

            if (tile == pointedTile) 
                return; // Already on pointed tile

            if(pointedTile)
                OnExitTile?.Invoke(this, pointedTile);
            
            pointedTile = tile;

            OnEnterTile?.Invoke(this, pointedTile);
        }
        else
        {
            if (!pointedTile) 
                return; // No pointed tile
            
            OnExitTile?.Invoke(this, pointedTile);
            pointedTile = null;
        }
    }

    /// <summary>
    /// Checks if player click on a tile (or element on a tile), previously checked by CheckRaycast().
    /// </summary>
    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0)) 
            return; // No click
        if (!pointedTile) 
            return; // Not on a tile

        if (pointedTile.character)
            OnClickOnCharacter?.Invoke(this, pointedTile.character);
        else
            OnClickOnTile?.Invoke(this, pointedTile);;
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
}
