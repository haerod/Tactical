using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static M__Managers;

public class M_PlayerInputs : MonoSingleton<M_PlayerInputs>
{
    [Header("INPUTS")]

    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;

    [Header("SCREEN MOUSE MOVEMENT")]
    [Range(1, 100)]
    [SerializeField] private int screenPercent = 5;
    [SerializeField] private int borderMultiplier = 1;

    //[HideInInspector] public Character c = null;

    private bool canClick = true;
    private Tile pointedTile;
    private List<Tile> currentPathfinding;
    private Character currentTarget;
    private int screenWidthPercented;
    private int screenHeightPercented;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        screenWidthPercented = Screen.width * screenPercent / 100;
        screenHeightPercented = Screen.height * screenPercent / 100;
    }

    private void Update()
    {
        if (!canClick) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        CheckRaycast();
        CheckClick();
        ChangeCharacter();
        CheckMouseScreenMovement();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ClearFeedbacksAndValues()
    {
        _feedbacks.DisableFeedbacks();
        _pathfinding.ClearPath();
        currentPathfinding = null;
        pointedTile = null;
        currentTarget = null;
    }

    public void SetClick(bool value = true)
    {
        canClick = value;

        if(value == false)
        {
            _feedbacks.SetCursor(M_Feedbacks.CursorType.Regular);
        }
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void ChangeCharacter()
    {
        if (Input.GetKeyDown(changeCharacterKey))
        {
            _characters.NextTurn();
        }
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pointedTile == null) return; // No on tile

            if(pointedTile.IsOccupied())
            {
                ClickAttack();
            }
            else
            {
                ClickMove();
            }
        }
    }

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

                if (tile.IsOccupied()) // Tile occupied by somebody
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

            _feedbacks.SetCursor(M_Feedbacks.CursorType.Regular);
            ClearFeedbacksAndValues();
        }
    }

    private void OnOccupiedTile(Tile tile)
    {
        _feedbacks.DisableFeedbacks();

        if (tile.Character() == _characters.currentCharacter) return; // Exit : same character
        if (tile.Character().Team() == _characters.currentCharacter.Team()) return; // Exit : same team

        currentTarget = tile.Character();

        if(_characters.currentCharacter.attack.HasSightOn(currentTarget))
        {
            if (_characters.currentCharacter.CanAttack())
            {
                _feedbacks.SetCursor(M_Feedbacks.CursorType.Aim);
            }
            else
            {
                _feedbacks.SetCursor(M_Feedbacks.CursorType.OutActionPoints);
            }
        }
        else
        {
            _feedbacks.SetCursor(M_Feedbacks.CursorType.OutAim);
        }
    }

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

        _feedbacks.SetCursor(M_Feedbacks.CursorType.Regular);
    }

    private void ClickAttack()
    {
        if (currentTarget == null) return; // Exit  : It's no target
        if (!_characters.currentCharacter.attack.HasSightOn(currentTarget)) return; // Exit : Isn't in sight

        _characters.currentCharacter.attack.AttackTarget(currentTarget, () => {
            if (_characters.IsVictory())
            {
                _characters.NextTurn();
            }
        });
    }

    private void ClickMove()
    {
        if (currentPathfinding == null) return; // Exit  : It's no path
        if (_characters.currentCharacter.actionPoints.actionPoints <= 0) return; // Exit : no action points aviable

        _characters.currentCharacter.move.MoveOnPath(currentPathfinding, () => { });
        _ui.DisableActionCostText();
    }

    private bool CanGo(Tile tile)
    {
        Character c = _characters.currentCharacter;

        if (!tile) return false; ;
        if (tile.type == Tile.Type.Hole) return false; ;
        if (tile.x == c.move.x && tile.y == c.move.y) return false;
        return true;
    }   
}
