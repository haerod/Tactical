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
    public int movementRange = 6;
    public List<TileType> walkableTiles;

    [Header("ANIMATION")]
    [Range(0,10f)]
    [SerializeField] private float speed = 6;
    [Range(0,1f)]
    [SerializeField] private float animSpeed = .5f;

    //[Header("EVENTS")]
    //[Space]
    //public UnityEvent onTileEnter;
    //[Space]

    [Header("COORDINATES (debug)")]
    public int x = 1;
    public int y = 1;
    
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
        c.SetCanPlayValue(false);

        if (c.movementRange <= 0) return;

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
        int range = 0;

        if(c.CanPlay()) 
            range = c.movementRange;

        currentArea = _pathfinding.AreaMovementZone(c.tile, range, c.move.walkableTiles);

        if (Utils.IsVoidList(currentArea)) return; // No way to go with current action points && position

        currentArea = currentArea.ToList();

        currentArea
            .Where(t => !t.IsOccupiedByCharacter())
            .ToList()
            .ForEach(t => t.SetMaterial(Tile.TileMaterial.Area));
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

        c.healthBar.GetComponentInParent<UI_OrientToCamera>().Orient();
    }

    /// <summary>
    /// Return true if the character can walk on this type of tile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <returns></returns>
    public bool CanWalkOn(TileType tileType)
    {
        return walkableTiles.Contains(tileType);
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
                //onTileEnter.Invoke();

                x = currentPath[index].x;
                y = currentPath[index].y;

                if (IsTheLastTile()) // EXIT : End path
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
    }

    /// <summary>
    /// Return true if is the last tile of the movement.
    /// </summary>
    /// <returns></returns>
    private bool IsTheLastTile()
    {
        if (index + 1 < currentPath.Count && c.movementRange > 0) return false;
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
}
