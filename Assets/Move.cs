using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class Move : MonoBehaviour
{
    [Header("COORDINATES")]
    public int x = 1;
    public int y = 1;

    [Header("GRID MOVE PARAMETERS")]
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
    [SerializeField] private Character c  = null;

    private bool move;
    private List<Tile> currentPath = null;
    private List<Tile> currentArea = null;
    private int index = 0;
    private Vector3 destination;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        c.transform.position = _terrain.grid[x, y].transform.position;
        OrientTo(orientation);
        anim.SetFloat("speed", 0f);
    }

    private void Update()
    {
        if (!move) return;

        MoveToDestination();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void MoveOnPath(List<Tile> path)
    {
        if (c.actionPoints.actionPoints <= 0) return;

        EndMove();
        move = true;

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        anim.SetFloat("speed", animSpeed); // Blend tree anim speed

        _inputs.canClick = false;

        ClearAreaZone();
    }

    public void EnableMoveArea()
    {
        Tile currentTile = _terrain.grid[x, y];
        int actionPoints = c.actionPoints.actionPoints;

        currentArea = _pathfinding.AreaMovementZone(currentTile, actionPoints);

        if (Utils.IsVoidList(currentArea)) return; // No way to go with current action points && position

        currentArea = currentArea.ToList();

        foreach (Tile t in currentArea)
        {
            if (t.IsOccupied()) continue;

            t.SetMaterial(Tile.TileMaterial.Area);
        }
    }

    public void ClearAreaZone()
    {
        if (currentArea == null) return;

        foreach (Tile t in currentArea)
        {
            t.SetMaterial(Tile.TileMaterial.Basic);
        }

        currentArea.Clear();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void MoveToDestination()
    {
        if (c.transform.position == destination)
        {
            onTileEnter.Invoke();
            x = currentPath[index].x;
            y = currentPath[index].y;
            NextTile();
        }
        else
        {
            c.transform.position = Vector3.MoveTowards(c.transform.position, destination, speed * Time.deltaTime);
        }
    }

    private void NextTile()
    {
        if (index + 1 < currentPath.Count && c.actionPoints.actionPoints > 0)
        {
            index++;
            destination = currentPath[index].transform.position;
            OrientTo(currentPath[index].transform.position);
            c.actionPoints.RemoveActionPoints();
        }
        else
        {
            EndMove();
        }
    }

    private void EndMove()
    {
        move = false;
        anim.SetFloat("speed", 0f);
        _inputs.canClick = true;
        EnableMoveArea();
    }

    private void OrientTo(Vector3 targetPosition, float offset = 0)
    {
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        endRotation *= Quaternion.Euler(new Vector3(0, offset, 0));
        c.transform.rotation = endRotation;
    }

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
