using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;

/// <summary>
/// Move an object on the board.
/// </summary>
public class MoveOnBoard : MonoBehaviour
{
    [SerializeField] private Transform movementTarget;
    [SerializeField] private Transform rotationTarget;
    private List<Tile> path;
    private int index;
    private Vector3 destination;
    public float speed;
    
    public event EventHandler<Tile> OnTileEnter;
    public event EventHandler OnMovementEnded;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void Move(List<Tile> newPath, float newSpeed)
    {
        path = newPath;
        
        if(path.Count == 0)
            return; // No path
        
        index = 0;
        destination = path[0].transform.position;
        OrientTo(path[0].transform.position);
        speed = newSpeed;

        StartCoroutine(Move());
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Moves the object to a destination and executes an action in the end.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Move()
    {
        while (true)
        {
            _camera.ResetPosition();
            
            if (movementTarget.position != destination) // Move
            {
                movementTarget.position = Vector3.MoveTowards(movementTarget.position, destination, speed * Time.deltaTime);
                yield return null;
            }
            else // On tile enter
            {
                OnTileEnter?.Invoke(this, path[index]);
                
                if (IsTheLastTile()) 
                {
                    OnMovementEnded?.Invoke(this, EventArgs.Empty);
                    yield break; // EXIT : End path
                }
                
                NextTile();
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// Changes the path to the next tile.
    /// </summary>
    private void NextTile()
    {
        index++;
        destination = path[index].transform.position;
        OrientTo(path[index].transform.position);
    }
    
    /// <summary>
    /// Returns true if is the last tile of the movement.
    /// </summary>
    /// <returns></returns>
    private bool IsTheLastTile() => index + 1 >= path.Count;
    
    /// <summary>
    /// Orients this object to another position, except on Y axis. Possibility to add an offset (euler angles).
    /// </summary>
    /// <param name="targetPosition"></param>
    private void OrientTo(Vector3 targetPosition)
    {
        Vector3 lookPos = targetPosition - movementTarget.position;
        lookPos.y = 0;
        Quaternion endRotation = Quaternion.Euler(Vector3.zero);
        if (lookPos != Vector3.zero)
            endRotation = Quaternion.LookRotation(lookPos);
        rotationTarget.rotation = endRotation;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}