using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class M_PlayerInputs : MonoBehaviour
{
    public static M_PlayerInputs inst;
    [HideInInspector] public bool canClick = true;

    [Header("INPUTS")]

    [SerializeField] private KeyCode changeCharacterKey = KeyCode.Tab;

    [Header("REFERENCES")]

    [SerializeField] private LineRenderer line = null;
    [SerializeField] private LineRenderer lineOut = null;
    [Range(.01f, .5f)]
    [SerializeField] private float lineOffset = .01f;

    [Space]
    [SerializeField] private Transform squareTransform = null;
    [Range(.01f, .5f)]
    [SerializeField] private float squareOffset = .01f;

    public Character c = null;

    [HideInInspector] public bool cValueChanged = false;

    private Camera cam;
    private TileStat currentTile;
    private List<TileStat> currentPathfinding;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
        else
        {
            Debug.LogError("2 managers !", this);
        }
    }

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
            M_Characters.instance.ChangeCharacter();
        }
    }

    private void CheckMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentPathfinding == null) return; // It's a path
            if (c.GetComponent<ActionPoints>().actionPoints <= 0) return; // It's action points aviable

            c.gridMove.MoveOnPath(currentPathfinding);
            M_UI.instance.DisableActionCostText();
        }
    }

    private void CheckRaycast()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileStat tile = hit.transform.GetComponent<TileStat>();

            if (!CanGo(tile)) // Isnt a tile, a hole tile or same tile
            {
                NoGo();
                return;
            }

            if (tile != currentTile || cValueChanged) // Next operations wasn't already done on this tile OR is another character
            {
                // New current tile and pathfinding
                currentTile = tile;
                currentPathfinding = M_Pathfinding.inst.Pathfind(c.gridMove.x, c.gridMove.y, tile.x, tile.y).ToList();

                SetLines();
                SetSquare();
            }
        }
        else
        {
            NoGo();
        }
    }

    private void NoGo()
    {
        squareTransform.gameObject.SetActive(false);
        line.gameObject.SetActive(false);
        lineOut.gameObject.SetActive(false);
        M_UI.instance.DisableActionCostText();
        M_Pathfinding.inst.ClearPath();
        currentPathfinding = null;
    }

    private bool CanGo(TileStat tile)
    {
        if (!tile) return false; ;
        if (tile.hole) return false; ;
        if (tile.x == c.gridMove.x && tile.y == c.gridMove.y) return false;        
        return true;
    }
    
    private void SetLines()
    {
        int actionPoints = c.actionPoints.actionPoints;

        // Target tile is in/out action points' range
        if (currentPathfinding.Count - 1 > actionPoints) // Out
        {
            line.positionCount = actionPoints + 1;
            lineOut.gameObject.SetActive(true);
            lineOut.positionCount = currentPathfinding.Count - actionPoints;
            M_UI.instance.SetActionCostText((currentPathfinding.Count - 1).ToString(), currentTile.transform.position, true);
        }   
        else // IN
        {
            lineOut.gameObject.SetActive(false);
            line.positionCount = currentPathfinding.Count;
            M_UI.instance.SetActionCostText((currentPathfinding.Count - 1).ToString(), currentTile.transform.position);
        }

        // Position line's points
        int i = 0;
        foreach (TileStat t in currentPathfinding)
        {
            if (i <= actionPoints)
            {
                line.SetPosition(i, t.transform.position + Vector3.up * lineOffset);
            }
            else
            {
                if (i == actionPoints + 1)
                {
                    lineOut.SetPosition(i - (actionPoints + 1), currentPathfinding[i - 1].transform.position + Vector3.up * lineOffset);
                }
                lineOut.SetPosition(i - (actionPoints), t.transform.position + Vector3.up * lineOffset);
            }

            i++;
        }

        line.gameObject.SetActive(true);
    }

    private void SetSquare()
    {
        squareTransform.gameObject.SetActive(true);
        squareTransform.position = currentTile.transform.position + Vector3.up * squareOffset;
    }
}
