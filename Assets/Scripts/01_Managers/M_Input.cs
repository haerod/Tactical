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

    public event EventHandler<Vector2Int> OnMovingCameraInput;
    public event EventHandler<int> OnZoomingCameraInput;

    private bool canClick = true;
    private Tile pointedTile;
    private List<Tile> currentPathfinding;
    private C__Character currentTarget;
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
    /// Clear feedbacks and null variables (path, target, etc.)
    /// </summary>
    public void ClearFeedbacksAndValues()
    {
        _feedback.DisableFreeTileFeedbacks();
        _ui.HidePercentText();
        currentPathfinding = null;
        pointedTile = null;
        currentTarget = null;
    }

    /// <summary>
    /// Set if players can click or not on board objects
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveClick(bool value = true)
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
            if (hit.transform.CompareTag("Clickable")) 
            {
                tile = hit.transform.GetComponentInParent<C__Character>().tile;
            }

            if (tile == pointedTile) 
                return; // Already on pointed tile

            if (!CanGo(tile) || !currentCharacter.CanPlay()) 
            {
                OnForbiddenTile();
                return; // Can't go on this tile or can't play
            }

            // NEXT STEP : Free tile or occupied tile

            pointedTile = tile;
            bool pointedCharacterIsVisible = 
                currentCharacter.look.VisibleTiles().Contains(tile)
                || _rules.enableFogOfWar == false;

            if (tile.IsOccupiedByCharacter()) // Tile occupied by somebody
            {
                if (pointedCharacterIsVisible)
                    OnOccupiedTile(tile);
                else
                    OnFreeTile(tile);
            }
            else // Free tile OR invisible tile
            {
                OnFreeTile(tile);
            }
        }
        else
        {
            OnForbiddenTile();
            return; // Out of tile board
        }
    }

    /// <summary>
    /// Check if player click on a tile (or element on a tile), previously checked by CheckRaycast().
    /// </summary>
    private void CheckClick()
    {
        if (!Input.GetMouseButtonDown(0)) return; // EXIT : No click
        if (!pointedTile) return; // EXIT : Not on a tile

        if (pointedTile.IsOccupiedByCharacter())
        {
            ClickAttack();
        }
        else
        {
            ClickMove();
        }
    }

    /// <summary>
    /// Check input to change character to another in the same team.
    /// </summary>
    private void CheckChangeCharacterInput()
    {
        if (!currentCharacter.behavior.playable) 
            return; // NPC turn.

        if (Input.GetKeyDown(changeCharacterKey))
        {
            _turns.SwitchToAnotherTeamPlayableCharacter();
        }
    }

    /// <summary>
    /// Check input to end turn and pass to the next team turn.
    /// </summary>
    private void CheckEndTurnInput()
    {
        if (Input.GetKeyDown(endTurnKey))
        {
            _turns.EndAllPlayableCharactersTurn();
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
    /// Check if mouse is on the borders (so have to move) or movement keys are pressed.
    /// </summary>
    private void CheckCameraMovementInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        bool upInput = mousePosition.y >= Screen.height || Input.GetKey(upKey);
        bool downInput = mousePosition.y <= 0 || Input.GetKey(downKey);
        bool leftInput = mousePosition.x <= 0 || Input.GetKey(leftKey);
        bool rightInput = mousePosition.x >= Screen.width || Input.GetKey(rightKey);
        Vector2Int direction = Vector2Int.zero;

        if (upInput)
            direction += Vector2Int.up;
        if (downInput)
            direction += Vector2Int.down;
        if (leftInput)
            direction += Vector2Int.left;
        if (rightInput)
            direction += Vector2Int.right;

        OnMovingCameraInput?.Invoke(this, direction);
    }

    /// <summary>
    /// Check if zoom input is pressed.
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

        // Mouse feedbacks depending of the line of sight on the enemy
        if (currentCharacter.look.HasSightOn(tile))
        {
            if (tile.character == currentCharacter || tile.character.team == currentCharacter.team) // Character or allie
            {
                _feedback.SetCursor(M_Feedback.CursorType.Regular);
                _ui.HidePercentText();
            }
            else // Enemy
            {
                _feedback.SetCursor(M_Feedback.CursorType.AimAndInSight);
                _ui.ShowPercentText(currentCharacter.attack.GetPercentToTouch(currentCharacter.look.LineOfSight(tile).Count));                
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
        // Disable fight
        _ui.HidePercentText();
        currentTarget = null;

        // Get pathfinding
        currentPathfinding = Pathfinding.Pathfind(
                        currentCharacter.tile,
                        tile,
                        Pathfinding.TileInclusion.WithStartAndEnd,
                        new MovementRules(currentCharacter.move.walkableTiles, currentCharacter.move.GetTraversableCharacterTiles()));

        if (currentPathfinding.Count == 0)
        {
            OnForbiddenTile(); 
            return; // EXIT : No path 
        }

        bool tileInMoveRange = currentCharacter.move.CanMoveTo(pointedTile);

        // Show movement feedbacks (square and line)
        _feedback.square.SetSquare(pointedTile.transform.position, tileInMoveRange);
        _feedback.line.SetLines(
            currentPathfinding,
            currentCharacter.move.movementRange, 
            pointedTile);

        // Set cursor
        _feedback.SetCursor(tileInMoveRange ? M_Feedback.CursorType.Regular : M_Feedback.CursorType.OutMovement);
    }

    /// <summary>
    /// When the pointer is on a hole, an obstacle, out of board or on a blocked destination.
    /// </summary>
    private void OnForbiddenTile()
    {
        _feedback.SetCursor(M_Feedback.CursorType.OutMovement);
        ClearFeedbacksAndValues();
    }

    // CLICKS
    // ======

    /// <summary>
    /// Actions happening if the player clicks on a tile occupied by another enemy character.
    /// Called by CheckClick().
    /// </summary>
    private void ClickAttack()
    {
        _ui.HidePercentText();

        if (!currentTarget) 
            return; // There is no target
        if(currentTarget.team == currentCharacter.team) 
            return; // Same team

        // Attack
        currentCharacter.attack.Attack(currentTarget);
    }

    /// <summary>
    /// Actions happening if the player clicks on a free tile to move.
    /// Called by CheckClick().
    /// </summary>
    private void ClickMove()
    {
        if (currentPathfinding == null) return; // EXIT : It's no path
        if (!currentCharacter.move.CanMoveTo(pointedTile)) return; // EXIT : click out of movement range

        currentCharacter.move.MoveOnPath(currentPathfinding);
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
        if (!tile) 
            return false; // No tile
        if (!currentCharacter.move.walkableTiles.Contains(tile.type)) 
            return false; // Unwalkable tile
        if (tile.x == currentCharacter.move.x && tile.y == currentCharacter.move.y) 
            return false; // Same tile
        
        return true;
    }   
}
