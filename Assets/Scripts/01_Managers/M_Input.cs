﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static M__Managers;

public class M_Input : MonoBehaviour
{
    [SerializeField] private KeyCode recenterCameraKey = KeyCode.Space;    
    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;
    [SerializeField] private KeyCode endTurnKey = KeyCode.Backspace;    

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
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_Input in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Update()
    {
        if (!canClick) return; // EXIT : Player can't click
        if (_ui.IsPointerOverUI()) return; // EXIT : Pointer over UI

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
            if (hit.transform.tag == "Clickable") 
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
        if (pointedTile == null) return; // EXIT : Not on a tile

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
        if (!currentCharacter.behavior.playable) return; // EXIT : NPC turn.

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
    /// Check if mouse is on the borders (so have to move).
    /// </summary>
    private void CheckMouseScreenMovement()
    {
        Vector3 direction = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;
        int borderMultiplier = _camera.borderMultiplier;

        if (mousePosition.x >= Screen.width - 1) // Right
        {
            if (mousePosition.x >= Screen.width)
                direction += _camera.transform.right * borderMultiplier;
            else
                direction += _camera.transform.right;
        }
        else if (mousePosition.x <= 1) // Left
        {
            if (mousePosition.x <= 0)
                direction -= _camera.transform.right * borderMultiplier;
            else
                direction -= _camera.transform.right;
        }

        if (mousePosition.y >= Screen.height - 1) // Up
        {
            if (mousePosition.y >= Screen.height)
                direction += _camera.transform.forward * borderMultiplier;
            else
                direction += _camera.transform.forward;
        }
        else if (mousePosition.y <= 1) // Down
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
        currentPathfinding = _pathfinding.Pathfind(
                        currentCharacter.tile,
                        tile,
                        M_Pathfinding.TileInclusion.WithStartAndEnd,
                        currentCharacter.move.Blockers(),
                        currentCharacter.move.MovementArea());

        if (currentPathfinding.Count == 0)
        {
            OnForbiddenTile(); 
            return; // No path 
        }

        bool tileInMoveRange = currentCharacter.move.CanMoveTo(pointedTile);

        // Show movement feedbacks (square and line)
        _feedback.square.SetSquare(pointedTile.transform.position, tileInMoveRange);
        _feedback.line.SetLines(
            currentPathfinding,
            currentCharacter.move.movementRange, 
            pointedTile);

        // Set cursor
        if (tileInMoveRange)
            _feedback.SetCursor(M_Feedback.CursorType.Regular);
        else
            _feedback.SetCursor(M_Feedback.CursorType.OutMovement);
    }

    /// <summary>
    /// When the pointer is on a hole, an obstacle, out of board or on a blocked destination.
    /// </summary>
    private void OnForbiddenTile()
    {
        _feedback.SetCursor(M_Feedback.CursorType.OutMovement);
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
        _ui.HidePercentText();

        if (currentTarget == null) return; // EXIT : There is no target
        if(currentTarget.team == currentCharacter.team) return; // EXIT : Same team

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
        if (!tile) return false;
        if (!currentCharacter.move.walkableTiles.Contains(tile.type)) return false;
        if (tile.x == currentCharacter.move.x && tile.y == currentCharacter.move.y) return false;
        return true;
    }   
}
