using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class M_Creator : MonoBehaviour
{
    [Header("SAVE")]

    public bool autoSave = true;

    [Header("SCREEN MOUSE MOVEMENT")]
    [Range(1, 100)]
    [SerializeField] private int screenPercent = 5;
    [SerializeField] private int borderMultiplier = 1;

    private int screenWidthPercented;
    private int screenHeightPercented;
    private Tile pointedTile;
    private Vector2Int newTileCoordinates;
    public static M_Creator instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_BoardCreator in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Start()
    {
        screenWidthPercented = Screen.width * screenPercent / 100;
        screenHeightPercented = Screen.height * screenPercent / 100;

        _ui.SetStartToggleValue();
    }

    private void Update()
    {
        if(UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return; // EXIT : Pointer over UI

        CheckRaycast();
        CheckMouseScreenMovement();
        ClickOnTile();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check if mouse is on the borders (so have to move).
    /// Copied from M_Inputs.
    /// Called by Update.
    /// </summary>
    /// <summary>
    /// Check if mouse is on the borders (so have to move).
    /// </summary>
    private void CheckMouseScreenMovement()
    {
        Vector3 direction = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= Screen.width - screenWidthPercented) // Right
        {
            if (mousePosition.x >= Screen.width)
                direction += _camera.transform.right * borderMultiplier;
            else
                direction += _camera.transform.right;
        }
        else if (mousePosition.x <= screenWidthPercented) // Left
        {
            if (mousePosition.x <= 0)
                direction -= _camera.transform.right * borderMultiplier;
            else
                direction -= _camera.transform.right;
        }

        if (mousePosition.y >= Screen.height - screenHeightPercented) // Up
        {
            if (mousePosition.y >= Screen.height)
                direction += _camera.transform.forward * borderMultiplier;
            else
                direction += _camera.transform.forward;
        }
        else if (mousePosition.y <= screenHeightPercented) // Down
        {
            if (mousePosition.y <= 0)
                direction -= _camera.transform.forward * borderMultiplier;
            else
                direction -= _camera.transform.forward;
        }

        if (direction == Vector3.zero) return;

        _camera.Move(direction);
    }

    /// <summary>
    /// Check over which object the pointer is (with raycast).
    /// Called by Update.
    /// </summary>
    private void CheckRaycast()
    {
        Camera cam = _camera.GetComponentInChildren<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.transform.GetComponent<Tile>();

            // On a character's collider, get the character's tile
            if (hit.transform.tag == "Clickable")
            {
                tile = hit.transform.GetComponentInParent<C__Character>().tile;
            }

            // EXIT : No tile;
            if (!tile) return;
            // EXIT : is on the already pointed tile (operation already done)
            if (tile == pointedTile) return;

            pointedTile = tile;

            _ui.SetDebugCoordinatesTextActive(true, pointedTile.x,pointedTile.y);
            _feedback.square.SetSquare(tile.transform.position);
        }
        else // Out of tile board
        {
            pointedTile = null;
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePoz = default;

            if (plane.Raycast(ray, out float distance))
            {
                mousePoz = ray.GetPoint(distance);
            }

            int xPoz = Mathf.RoundToInt(mousePoz.x);
            int zPoz = Mathf.RoundToInt(mousePoz.z);

            Vector3 newPoz = new Vector3(xPoz, 0, zPoz);
            newTileCoordinates = new Vector2Int(xPoz, zPoz);

            _feedback.square.SetSquare(newPoz);

            _ui.SetDebugCoordinatesTextActive(true, xPoz, zPoz);
        }
    }

    /// <summary>
    /// When the player clicks on tile.
    /// Called by Update.
    /// </summary>
    private void ClickOnTile()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Click on tile : modify tile's type
            if (pointedTile) 
            {
                int current = (int)pointedTile.type;
                current = current.Next(Enum.GetNames(typeof(Tile.Type)).Length -1);
                pointedTile.ChangeType((Tile.Type) current);
            }
            // Click on void tile : create a new tile
            else
            {
                _board.CreateTileAtCoordinates(newTileCoordinates.x, newTileCoordinates.y);
            }

            if(autoSave)
            {
                _board.SaveBoard();
            }
        }
    }
}
