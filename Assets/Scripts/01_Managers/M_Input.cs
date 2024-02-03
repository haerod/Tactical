using UnityEngine;
using UnityEngine.EventSystems;
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
        _ui.percentText.DisablePercentShootText();
        _pathfinding.ClearPath();
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

            if (tile == pointedTile) return; // EXIT : is on the already pointed tile (operation already done)

            if (!CanGo(tile)) // EXIT : Is a hole tile, big obstacle tile or current character's tile
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
        else // EXIT : Out of tile board
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
    /// Check input to change character to another in the same team.
    /// </summary>
    private void CheckChangeCharacterInput()
    {
        if (!_characters.current.behavior.playable) return; // EXIT : NPC turn.

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
        C__Character c = _characters.current;

        // EXIT : same character
        if (tile.Character() == c) return;
        // EXIT : same team
        if (tile.Character().Team() == c.Team()) return; 

        // Mouse feedbacks depending of the line of sight on the enemy
        if(c.look.HasSightOn(tile))
        {
            if (c.CanPlay())
            {
                _feedback.SetCursor(M_Feedback.CursorType.AimAndInSight);
                _ui.percentText.SetPercentShootText(c.attack.GetPercentToTouch(c.look.LineOfSight(tile).Count));
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
        C__Character currentCharacter = _characters.current;

        _ui.percentText.DisablePercentShootText();

        currentTarget = null;

        if(currentCharacter.CanPlay())
            currentPathfinding = _pathfinding.Pathfind(
                            _characters.current.tile,
                            tile,
                            M_Pathfinding.TileInclusion.WithEnd,
                            currentCharacter.move.walakbleTiles);

        // EXIT : No path
        if (Utils.IsVoidList(currentPathfinding))
        {
            OnForbiddenTile();
            return;
        }

        int range;

        if (currentCharacter.CanPlay())
            range = currentCharacter.movementRange;
        else
            range = 0;

        bool tileInMoveRange = (currentPathfinding.Count - 1) <= range;

        _feedback.square.SetSquare(pointedTile.transform.position, tileInMoveRange);
        _feedback.line.SetLines(
            currentPathfinding,
            currentCharacter, 
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
        C__Character c = _characters.current;
        _ui.percentText.DisablePercentShootText();

        if (currentTarget == null) return; // EXIT : There is no target

        // Attack
        c.attack.Attack(currentTarget);
    }

    /// <summary>
    /// Actions happening if the player clicks on a free tile to move.
    /// Called by CheckClick().
    /// </summary>
    private void ClickMove()
    {
        if (currentPathfinding == null) return; // EXIT : It's no path
        if (_characters.current.movementRange <= 0) return; // EXIT : no action points aviable

        _characters.current.move.MoveOnPath(currentPathfinding, () =>
        {
            _turns.EndTurn();
        });
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
        C__Character c = _characters.current;

        if (!tile) return false;
        if (!c.move.walakbleTiles.Contains(tile.type)) return false;
        if (tile.x == c.move.x && tile.y == c.move.y) return false;
        return true;
    }   
}
