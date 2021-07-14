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

    [HideInInspector] public Character c = null;
    [HideInInspector] public bool cValueChanged = false;

    private bool canClick = true;
    private Camera cam;
    private Tile pointedTile;
    private List<Tile> currentPathfinding;
    private Character currentTarget;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (canClick && !EventSystem.current.IsPointerOverGameObject()) // can click and not over UI
        {
            ChangeCharacter();
            CheckRaycast();
            CheckClick();
        }

        cValueChanged = false;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ClearFeedbacksAndValues()
    {
        ClearFeedbacks();
        _pathfinding.ClearPath();
        currentPathfinding = null;
        pointedTile = null;
        currentTarget = null;
    }

    public void SetClick(bool value = true)
    {
        canClick = value;
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

    private void CheckRaycast()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.transform.GetComponent<Tile>();
    
            if (!CanGo(tile)) // Isn't a tile, a hole tile or same tile
            {
                ClearFeedbacksAndValues();
                return;
            }

            if (tile != pointedTile || cValueChanged) // Next operations wasn't already done on this tile OR is another character
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

            ClearFeedbacksAndValues();
        }
    }

    private void ClearFeedbacks()
    {
        _feedbacks.square.DisableSquare();
        _feedbacks.line.DisableLines();
        _ui.DisableActionCostText();
    }

    private void CheckLignOfSight()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return; // Exit : no raycast

        Tile tile = hit.transform.GetComponent<Tile>();


        //if (!tile.IsOccupied()) return; // Exit : Free tile
        //if (tile == _characters.currentCharacter.GetTile()) // Exit : current character's tile        
        //currentLoS = _characters.currentCharacter.attack.LineOfSight(tile);
    }

    private void OnOccupiedTile(Tile tile)
    {
        ClearFeedbacks();

        if (tile.Character() == c) return; // Exit : same character
        // TO DO LATER : CHECK SQUAD
        currentTarget = tile.Character();

        // Move Around Behavior
        // ====================

        //currentPathfinding = _pathfinding.PathfindAround(
        //    c.Tile(),
        //    tile,
        //    _rules.canPassAcross == M_GameRules.PassAcross.Nobody);

        //if (Utils.IsVoidList(currentPathfinding))
        //{
        //    ClearFeedbacksAndValues();
        //    return;
        //}

        //currentPathfinding = currentPathfinding.ToList();
        //Tile endTile = currentPathfinding.LastOrDefault();
        //_feedbacks.square.SetSquare(endTile);
        //_feedbacks.line.SetLines(currentPathfinding, c, endTile);
    }

    private void OnFreeTile(Tile tile)
    {
        currentTarget = null;

        currentPathfinding = _pathfinding.Pathfind(
                        c.Tile(),
                        tile);

        if (Utils.IsVoidList(currentPathfinding)) // No path
        {
            ClearFeedbacksAndValues();
            return;
        }

        currentPathfinding = currentPathfinding.ToList();
        _feedbacks.square.SetSquare(pointedTile);
        _feedbacks.line.SetLines(currentPathfinding, c, pointedTile);
    }

    private void ClickAttack()
    {
        if (currentTarget == null) return; // Exit  : It's no target
        if (!c.attack.HasSightOn(currentTarget)) return; // Exit : Isn't in sight

        c.attack.AttackTarget(currentTarget, () => { });
    }

    private void ClickMove()
    {
        if (currentPathfinding == null) return; // Exit  : It's no path
        if (c.actionPoints.actionPoints <= 0) return; // Exit : no action points aviable

        c.move.MoveOnPath(currentPathfinding, () => { });
        _ui.DisableActionCostText();
    }

    private bool CanGo(Tile tile)
    {
        if (!tile) return false; ;
        if (tile.hole) return false; ;
        if (tile.x == c.move.x && tile.y == c.move.y) return false;
        return true;
    }   
}
