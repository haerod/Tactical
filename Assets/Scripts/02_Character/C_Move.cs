using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;
using UnityEditor;

public class C_Move : MonoBehaviour
{
    [Header("COORDINATES")]
    public int x = 1;
    public int y = 1;

    [Header("GRID MOVE PARAMETERS")]
    public List<TileType> walakbleTiles;
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    [Range(0,1f)]
    [SerializeField] private float animSpeed = .5f;
    public enum Orientation { North, NorthEast, East, SouthEst, South, SouthWest, West, NorthWest}
    public Orientation orientation;

    [Header("EVENTS")]
    [Space]
    public UnityEvent onTileEnter;
    [Space]
    
    [Header("REFERENCES")]

    [SerializeField] private Animator anim = null;
    [SerializeField] private C__Character c  = null;

    private List<Tile> currentPath = null;
    private List<Tile> currentArea = null;
    private int index = 0;
    private Vector3 destination;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Start the movement on a path, with and action on end of this path.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="OnEnd"></param>
    public void MoveOnPath(List<Tile> path, Action OnEnd)
    {
        if (c.actionPoints.movementRange <= 0) return;

        EndMove();

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        anim.SetFloat("speed", animSpeed); // Blend tree anim speed

        _input.SetActiveClick(false);
        _ui.SetActivePlayerUI_Action(false);

        c.ClearTilesFeedbacks();

        StartCoroutine(MoveToDestination(() => OnEnd()));
    }

    /// <summary>
    /// Show the tiles of the movement area.
    /// </summary>
    public void EnableMoveArea()
    {
        int actionPoints = c.actionPoints.movementRange;

        currentArea = _pathfinding.AreaMovementZone(c.tile, actionPoints, c.move.walakbleTiles);

        if (Utils.IsVoidList(currentArea)) return; // No way to go with current action points && position

        currentArea = currentArea.ToList();

        foreach (Tile t in currentArea)
        {
            if (t.IsOccupiedByCharacter()) continue;

            t.SetMaterial(Tile.TileMaterial.Area);
        }
    }

    /// <summary>
    /// Reset the material of the tiles in the area zone and clear the list.
    /// </summary>
    public void ClearAreaZone()
    {
        if (currentArea == null) return;

        foreach (Tile t in currentArea)
        {
            t.ResetTileSkin();
        }

        currentArea.Clear();
    }

    /// <summary>
    /// Orient this object to another position, except on Y axis. Possibility to add an offset (euler angles).
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="offset"></param>
    public void OrientTo(Vector3 targetPosition, float offset = 0)
    {
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        endRotation *= Quaternion.Euler(new Vector3(0, offset, 0));
        c.transform.rotation = endRotation;

        c.healthBar.GetComponentInParent<OrientToCamera>().Orient();
    }

    /// <summary>
    /// Orient character at its creation.
    /// Called by M_Board.
    /// </summary>
    public void OrientToBasicPosition()
    {
        OrientTo(orientation);
        anim.SetFloat("speed", 0f);
        EditorUtility.SetDirty(c.anim); // Save the character modifications
        EditorUtility.SetDirty(c.transform); // Save the character modifications
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Move the object to a desination and execute an action in the end.
    /// </summary>
    /// <param name="OnEnd"></param>
    /// <returns></returns>
    IEnumerator MoveToDestination(Action OnEnd)
    {
        while (true)
        {
            _camera.ResetPosition();

            if (c.transform.position != destination) // Move
            {
                c.transform.position = Vector3.MoveTowards(c.transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
            else // On tile enter
            {
                onTileEnter.Invoke();

                x = currentPath[index].x;
                y = currentPath[index].y;

                if (IsEnd()) // Exit : End path
                {
                    EndMove(() => OnEnd());
                    yield break;
                }
                else
                {
                    NextTile();
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// Happend when a character move to the next tile.
    /// </summary>
    private void NextTile()
    {
        index++;
        destination = currentPath[index].transform.position;
        OrientTo(currentPath[index].transform.position);
        c.actionPoints.RemoveActionPoints();
    }

    /// <summary>
    /// Return true if is the last tile of the movement.
    /// </summary>
    /// <returns></returns>
    private bool IsEnd()
    {
        if (index + 1 < currentPath.Count && c.actionPoints.movementRange > 0) return false;
        return true;
    }

    /// <summary>
    /// Happend in the end of the movement.
    /// </summary>
    /// <param name="OnEnd"></param>
    private void EndMove(Action OnEnd = default)
    {
        anim.SetFloat("speed", 0f);
        _input.SetActiveClick();
        _ui.SetActivePlayerUI_Action(true);

        if (_characters.current.behavior.playable)
        {
            c.EnableTilesFeedbacks();
        }

        if (OnEnd == default) OnEnd = (() => { });

        OnEnd();
    }

    /// <summary>
    /// Orient to the defined oriention (cardinal points).
    /// </summary>
    /// <param name="o"></param>
    private void OrientTo(Orientation o)
    {
        switch (o)
        {
            case Orientation.North:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
                break;
            case Orientation.NorthEast:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case Orientation.East:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
                break;
            case Orientation.SouthEst:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case Orientation.South:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 245, 0));
                break;
            case Orientation.SouthWest:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                break;
            case Orientation.West:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
                break;
            case Orientation.NorthWest:
                c.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

}
