using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using static M__Managers;

public class M_PlayerInputs : MonoSingleton<M_PlayerInputs>
{
    [HideInInspector] public bool canClick = true;

    [Header("INPUTS")]

    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;

    [HideInInspector] public Character c = null;
    [HideInInspector] public bool cValueChanged = false;

    private Camera cam;
    private Tile pointedTile;
    private List<Tile> currentPathfinding;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        ChangeCharacter();

        if (canClick && !EventSystem.current.IsPointerOverGameObject()) // can click and not over UI
        {
            CheckRaycast();
            CheckMove();
        }

        cValueChanged = false;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void ChangeCharacter()
    {
        if (Input.GetKeyDown(changeCharacterKey))
        {
            _characters.ChangeCharacter();
        }
    }

    private void CheckMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentPathfinding == null) return; // It's a path
            if (c.GetComponent<ActionPoints>().actionPoints <= 0) return; // It's action points aviable

            c.gridMove.MoveOnPath(currentPathfinding);
            _ui.DisableActionCostText();
        }
    }

    private void CheckRaycast()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.transform.GetComponent<Tile>();

            if (!CanGo(tile)) // Isn't a tile, a hole tile or same tile
            {
                NoGo();
                return;
            }

            if (tile != pointedTile || cValueChanged) // Next operations wasn't already done on this tile OR is another character
            {
                // New current tile and pathfinding
                pointedTile = tile;
                Tile characterTile = _terrain.grid[c.gridMove.x, c.gridMove.y];

                if (tile.IsOccupied()) // Tile occupied by somebody
                {
                    currentPathfinding = _pathfinding.PathfindAround(
                        characterTile,
                        tile,
                        _rules.canPassAcross == M_GameRules.PassAcross.Nobody);

                    if (Utils.IsVoidList(currentPathfinding))
                    {
                        NoGo();
                        return;
                    }

                    currentPathfinding = currentPathfinding.ToList();
                    Tile endTile = currentPathfinding.LastOrDefault();
                    _feedbacks.square.SetSquare(endTile);
                    _feedbacks.line.SetLines(currentPathfinding, c, endTile);
                }
                else // Free tile
                {
                    currentPathfinding = _pathfinding.Pathfind(
                        characterTile,
                        tile);

                    if (Utils.IsVoidList(currentPathfinding))
                    {
                        NoGo();
                        return;
                    }

                    currentPathfinding = currentPathfinding.ToList();
                    _feedbacks.square.SetSquare(pointedTile);
                    _feedbacks.line.SetLines(currentPathfinding, c, pointedTile);
                }
            }
        }
        else
        {
            NoGo();
        }
    }

    private void NoGo()
    {
        _feedbacks.square.DisableSquare();
        _feedbacks.line.DisableLines();
        _ui.DisableActionCostText();
        _pathfinding.ClearPath();
        currentPathfinding = null;
        pointedTile = null;
    }

    private bool CanGo(Tile tile)
    {
        if (!tile) return false; ;
        if (tile.hole) return false; ;
        if (tile.x == c.gridMove.x && tile.y == c.gridMove.y) return false;
        return true;
    }   
}
