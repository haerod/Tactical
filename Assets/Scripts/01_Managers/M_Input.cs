using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static M__Managers;

public class M_Input : MonoBehaviour
{
    [Header("INPUT")]

    [SerializeField] private KeyCode recenterCameraKey = KeyCode.Space;    
    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;
    [SerializeField] private KeyCode endTurnKey = KeyCode.Backspace;    

    [Header("SCREEN MOUSE MOVEMENT")]
    [Range(1, 100)]
    [SerializeField] private int screenPercent = 5;
    [SerializeField] private int borderMultiplier = 1;

    private bool canClick = true;
    private Tile pointedTile;
    private List<Tile> currentPathfinding;
    private C__Character currentTarget;
    private int screenWidthPercented;
    private int screenHeightPercented;
    public static M_Input instance;

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
            Debug.LogError("There is more than one M_Input in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Start()
    {
        screenWidthPercented = Screen.width * screenPercent / 100;
        screenHeightPercented = Screen.height * screenPercent / 100;
    }

    private void Update()
    {
        if (!canClick) return; // EXIT : Player can't click
        if (EventSystem.current.IsPointerOverGameObject()) return; // EXIT : Pointer over UI

        CheckRaycast();
        CheckClick();
        CheckChangeCharacterInput();
        CheckEndTurnInput();
        CheckRecenterCameraInput();
        CheckMouseScreenMovement();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Clear feedbacks and null variables (path, target, etc.)
    /// </summary>
    public void ClearFeedbacksAndValues()
    {
        _feedback.DisableFreeTileFeedbacks();
        _feedback.percentText.DisablePercentShootText();
        _pathfinding.ClearPath();
        currentPathfinding = null;
        pointedTile = null;
        currentTarget = null;
    }

    /// <summary>
    /// Set if players can click or not on board objects
    /// </summary>
    /// <param name="value"></param>
    public void SetClick(bool value = true)
    {
        canClick = value;

        if(value == false)
        {
            _feedback.SetCursor(M_Feedback.CursorType.Regular);
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // CHECKERS
    // ========

    /// <summary>
    /// Check over which object the pointer is (with raycast).
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

            // EXIT : is on the already pointed tile (operation already done)
            if (tile == pointedTile) return;

            // EXIT : Is a hole tile, big obstacle tile or current character's tile
            if (!CanGo(tile)) 
            {
                OnForbiddenTile();
                return;
            }

            // NEXT STEP : Free tile or occupied tile

            pointedTile = tile;

            if (tile.IsOccupiedByCharacter()) // Tile occupied by somebody
            {
                OnOccupiedTile(tile);
            }
            else // Free tile
            {
                OnFreeTile(tile);
            }
        }
        // EXIT : Out of tile board
        else
        {
            OnForbiddenTile();
            return;
        }
    }

    /// <summary>
    /// Check if player click on a tile (or element on a tile), previously checked by CheckRaycast().
    /// </summary>
    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // EXIT : Not on a tile
            if (pointedTile == null) return; 

            if(pointedTile.IsOccupiedByCharacter())
            {
                ClickAttack();
            }
            else
            {
                ClickMove();
            }
        }
    }

    /// <summary>
    /// Check input to change character to another.
    /// </summary>
    private void CheckChangeCharacterInput()
    {
        if (Input.GetKeyDown(changeCharacterKey))
        {
            _characters.NextTeamCharacter();
        }
    }

    /// <summary>
    /// Check input to end turn and pass to the next team turn.
    /// </summary>
    private void CheckEndTurnInput()
    {
        if (Input.GetKeyDown(endTurnKey))
        {
            _characters.NextTurn();
        }
    }

    /// <summary>
    /// Check input to recenter camera on the current character
    /// </summary>
    private void CheckRecenterCameraInput()
    {
        if (Input.GetKeyDown(recenterCameraKey))
        {
            _camera.ResetPosition();
        }
    }

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

    // ON TILE
    // =======

    /// <summary>
    /// Actions happening if the pointer overlaps an occupied tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnOccupiedTile(Tile tile)
    {
        _feedback.DisableFreeTileFeedbacks();

        currentTarget = tile.Character();
        C__Character c = _characters.currentCharacter;

        // EXIT : same character
        if (tile.Character() == c) return;
        // EXIT : same team
        if (tile.Character().Team() == c.Team()) return; 

        // Mouse feedbacks depending of the line of sight on the enemy
        if(c.look.HasSightOn(tile))
        {
            if (c.CanAttack())
            {
                _feedback.SetCursor(M_Feedback.CursorType.AimAndInSight);
                _feedback.percentText.SetPercentShootText(c.attack.GetPercentToTouch(c.look.LineOfSight(tile).Count));
            }
            else
            {
                _feedback.SetCursor(M_Feedback.CursorType.OutActionPoints);
            }
        }
        else
        {
            _feedback.SetCursor(M_Feedback.CursorType.OutAimOrSight);
        }
    }

    /// <summary>
    /// Actions happening if the pointer overlaps a free tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnFreeTile(Tile tile)
    {
        _feedback.percentText.DisablePercentShootText();

        currentTarget = null;

        currentPathfinding = _pathfinding.Pathfind(
                        _characters.currentCharacter.tile,
                        tile);

        // EXIT : No path
        if (Utils.IsVoidList(currentPathfinding))
        {
            OnForbiddenTile();
            return;
        }

        _feedback.square.SetSquare(pointedTile.transform.position);
        _feedback.line.SetLines(
            currentPathfinding, 
            _characters.currentCharacter, 
            pointedTile);

        _feedback.SetCursor(M_Feedback.CursorType.Regular);
    }

    /// <summary>
    /// When the pointer is on a hole, an obstacle, out of board or on a blocked destination.
    /// </summary>
    private void OnForbiddenTile()
    {
        _feedback.SetCursor(M_Feedback.CursorType.Regular);
        ClearFeedbacksAndValues();
    }

    // CLICS
    // =====

    /// <summary>
    /// Actions happening if the player clicks on a tile occupied by another enemy character.
    /// Called by CheckClick().
    /// </summary>
    private void ClickAttack()
    {
        C__Character c = _characters.currentCharacter;
        _feedback.percentText.DisablePercentShootText();

        // EXIT : There is no target
        if (currentTarget == null) return;

        // Attack
        c.attack.AttackTarget(currentTarget, () => {
            if (_characters.IsFinalTeam(c))
            {
                _characters.NextTurn();
            }
        });
    }

    /// <summary>
    /// Actions happening if the player clicks on a free tile to move.
    /// Called by CheckClick().
    /// </summary>
    private void ClickMove()
    {
        // EXIT : It's no path
        if (currentPathfinding == null) return;
        // EXIT : no action points aviable
        if (_characters.currentCharacter.actionPoints.actionPoints <= 0) return; 

        _characters.currentCharacter.move.MoveOnPath(currentPathfinding, () => { });
        _feedback.actionCostText.DisableActionCostText();
    }

    // OTHER
    // =====

    /// <summary>
    /// Return true if the character can go on this tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool CanGo(Tile tile)
    {
        C__Character c = _characters.currentCharacter;

        if (!tile) return false; ;
        if (tile.type == Tile.Type.Hole) return false;
        if (tile.type == Tile.Type.BigObstacle) return false;
        if (tile.x == c.move.x && tile.y == c.move.y) return false;
        return true;
    }   
}
