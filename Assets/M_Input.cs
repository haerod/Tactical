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
    private Character currentTarget;
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
        if (!canClick) return;
        if (EventSystem.current.IsPointerOverGameObject()) return; // Return if pointer over UI

        CheckRaycast();
        CheckClick();
        CheckChangeCharacterInput();
        CheckMouseScreenMovement();
        CheckRecenterCameraInput();
        CheckEndTurnInput();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Clear feedbacks and null variables (path, target, etc.)
    /// </summary>
    public void ClearFeedbacksAndValues()
    {
        _feedbacks.DisableFeedbacks();
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
            _feedbacks.SetCursor(M_Feedback.CursorType.Regular);
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

            if (hit.transform.tag == "Clickable")
            {
                tile = hit.transform.GetComponentInParent<Character>().Tile();
            }
    
            if (!CanGo(tile)) // Isn't a tile, a hole tile or same tile
            {
                ClearFeedbacksAndValues();
                return;
            }

            if (tile != pointedTile ) // Next operations wasn't already done on this tile OR is another character
            {
                // New current tile and pathfinding
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
        }
        else
        {
            if (pointedTile == null) return;

            _feedbacks.SetCursor(M_Feedback.CursorType.Regular);
            ClearFeedbacksAndValues();
        }
    }

    /// <summary>
    /// Check if player click on a tile (or element on a tile), previously checked by CheckRaycast().
    /// </summary>
    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pointedTile == null) return; // Exit : Not on a tile

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
            _characters.NextTurn();
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
                direction += _camera.transform.up * borderMultiplier;
            else
                direction += _camera.transform.up;
        }
        else if (mousePosition.y <= screenHeightPercented) // Down
        {
            if (mousePosition.y <= 0)
                direction -= _camera.transform.up * borderMultiplier;
            else
                direction -= _camera.transform.up;
        }

        if (direction == Vector3.zero) return;

        _camera.Move(direction);
    }

    // OTHERS
    // ======

    /// <summary>
    /// Actions happening if the pointer overlaps an occupied tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnOccupiedTile(Tile tile)
    {
        _feedbacks.DisableFeedbacks();

        currentTarget = tile.Character();

        // Mouse feedbacks
        if (tile.Character() == _characters.currentCharacter) return; // Exit : same character
        if (tile.Character().Team() == _characters.currentCharacter.Team()) return; // Exit : same team

        if(_characters.currentCharacter.look.HasSightOn(currentTarget.Tile()))
        {
            if (_characters.currentCharacter.CanAttack())
            {
                _feedbacks.SetCursor(M_Feedback.CursorType.AimOrInSight);
            }
            else
            {
                _feedbacks.SetCursor(M_Feedback.CursorType.OutActionPoints);
            }
        }
        else
        {
            print("out aim");
            _feedbacks.SetCursor(M_Feedback.CursorType.OutAimOrSight);
        }
    }

    /// <summary>
    /// Actions happening if the pointer overlaps a free tile.
    /// </summary>
    /// <param name="tile"></param>
    private void OnFreeTile(Tile tile)
    {
        currentTarget = null;

        currentPathfinding = _pathfinding.Pathfind(
                        _characters.currentCharacter.Tile(),
                        tile);

        if (Utils.IsVoidList(currentPathfinding)) // No path
        {
            ClearFeedbacksAndValues();
            return;
        }

        currentPathfinding = currentPathfinding.ToList();
        _feedbacks.square.SetSquare(pointedTile);
        _feedbacks.line.SetLines(
            currentPathfinding, 
            _characters.currentCharacter, 
            pointedTile);

        _feedbacks.SetCursor(M_Feedback.CursorType.Regular);
    }

    /// <summary>
    /// Actions happening if the player clicks on a tile occupied by another enemy character.
    /// </summary>
    private void ClickAttack()
    {
        if (currentTarget == null) return; // Exit  : It's no target
        if (!_characters.currentCharacter.look.HasSightOn(currentTarget.Tile())) return; // Exit : Isn't in sight

        _characters.currentCharacter.attack.AttackTarget(currentTarget, () => {
            if (_characters.IsFinalTeam(_characters.currentCharacter))
            {
                _characters.NextTurn();
            }
        });
    }

    /// <summary>
    /// Actions happening if the player clicks on a free tile to move.
    /// </summary>
    private void ClickMove()
    {
        if (currentPathfinding == null) return; // Exit  : It's no path
        if (_characters.currentCharacter.actionPoints.actionPoints <= 0) return; // Exit : no action points aviable

        _characters.currentCharacter.move.MoveOnPath(currentPathfinding, () => { });
        _ui.DisableActionCostText();
    }

    /// <summary>
    /// Return true if the character can go on this tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool CanGo(Tile tile)
    {
        Character c = _characters.currentCharacter;

        if (!tile) return false; ;
        if (tile.type == Tile.Type.Hole) return false; ;
        if (tile.x == c.move.x && tile.y == c.move.y) return false;
        return true;
    }   
}
