using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class GridMove : MonoBehaviour
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

    [SerializeField] private TiledTerrainCreator terrain = null;
    [SerializeField] private Animator anim = null;
    [SerializeField] private Character c  = null;

    private bool move;
    private List<TileStat> currentPath = null;
    private int index = 0;
    private Vector3 destination;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        transform.position = terrain.grid[x, y].transform.position;
        OrientTo(orientation);
        anim.SetFloat("speed", 0f);
    }

    private void Update()
    {
        if (!move) return;

        MoveOnPath();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void MoveOnPath(List<TileStat> path)
    {
        if (c.actionPoints.actionPoints <= 0) return;

        EndMove();
        move = true;

        currentPath = path.ToList();

        index = 0;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);

        anim.SetFloat("speed", animSpeed); // Blend tree anim speed

        M_PlayerInputs.inst.canClick = false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void MoveOnPath()
    {
        if (transform.position == destination)
        {
            onTileEnter.Invoke();
            x = currentPath[index].x;
            y = currentPath[index].y;
            NextTile();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
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
        M_PlayerInputs.inst.canClick = true;
    }

    private void OrientTo(Vector3 targetPosition, float offset = 0)
    {
        Vector3 lookPos = targetPosition - transform.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        endRotation *= Quaternion.Euler(new Vector3(0, offset, 0));
        transform.rotation = endRotation;
    }

    private void OrientTo(Orientation o)
    {
        switch (o)
        {
            case Orientation.North:
                transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
                break;
            case Orientation.NorthEast:
                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case Orientation.East:
                transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
                break;
            case Orientation.SouthEst:
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case Orientation.South:
                transform.rotation = Quaternion.Euler(new Vector3(0, 245, 0));
                break;
            case Orientation.SouthWest:
                transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                break;
            case Orientation.West:
                transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
                break;
            case Orientation.NorthWest:
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

}
