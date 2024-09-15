using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[ExecuteInEditMode]
public class CoverAutoSnap : MonoBehaviour
{
    [HideInInspector] public M_Board board; // Note : Let it serializable to be dirty.
    public bool isLocated; // Note : Let it serializable to be dirty.
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private Vector3 gizmoSize = Vector3.one;
    [SerializeField] private Vector3 gizmoOffset = Vector3.zero;
    public bool tested;

    private void Start()
    {
        board = FindAnyObjectByType<M_Board>();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    private void Update()
    {
        if (!IsInEditor())
            return; // Not in editor
        if (!transform.hasChanged)
            return; // Didnt move
        if (!IsOnValidPosition())
            return; // Not a valid position

        float decimalsX = transform.position.x % 1;
        float decimalsY = transform.position.z % 1;
        float xDelta = Mathf.Abs(1 - decimalsX);
        float yDelta = Mathf.Abs(1 - decimalsY);

        Vector2 coordinates = new Vector2();

        if (xDelta > yDelta)
        {
            coordinates.x = Mathf.RoundToInt(transform.position.x);
            coordinates.y = RoundToHalf(transform.position.z);
        }
        else
        {
            coordinates.x = RoundToHalf(transform.position.x);
            coordinates.y = Mathf.RoundToInt(transform.position.z);
        }

        if (GetOtherCoverAtPosition(new Vector3(coordinates.x, 0, coordinates.y)))
        {
            isLocated = false;
            return; // Already a cover at this position
        }

        MoveObject(coordinates);
        AutoRotateObject();
    }

    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            return; // Prefab mode
        if (isLocated)
            return; // Is located

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position + gizmoOffset, gizmoSize);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Move the object and setup parameters if necessary.
    /// </summary>
    private void MoveObject(Vector2 coordinates)
    {
        transform.position = new Vector3(coordinates.x, 0, coordinates.y);
        isLocated = true;
    }

    /// <summary>
    /// Round to the closest value + 0.5 (ex : 0.5, 1.5, 2.5, ...).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float RoundToHalf(float value) => Mathf.Sign(value) * (Mathf.Abs((int)value) + 0.5f);

    private void AutoRotateObject()
    {
        if (transform.position.x % 1 == 0)
            transform.eulerAngles = Vector3.up * 90f;
        else
            transform.eulerAngles = Vector3.zero;
    }

    private bool IsInEditor()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return false; // Play mode
        if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            return false; // Prefab mode

        return true;
    }

    private bool IsOnValidPosition()
    {
        Tile validTile = GetTileUnder();

        if (!validTile)
        {
            isLocated = false;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Get the tile under the character, if it can walk on.
    /// </summary>
    /// <returns></returns>
    private Tile GetTileUnder()
    {
        Collider[] colliderTileArray = Physics.OverlapSphere(transform.position, .1f);

        foreach (Collider colliderTile in colliderTileArray)
        {
            Tile testedTile = colliderTile.GetComponent<Tile>();

            if (!testedTile)
                continue; // No tile
            //if (!character.move.CanWalkOn(testedTile.type))
            //    continue; // Not walkable

            return testedTile;
        }

        return null;
    }

    /// <summary>
    /// Get other cover at position if it's not itself.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool GetOtherCoverAtPosition(Vector3 testedPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(testedPosition, .1f);

        foreach (Collider collider in colliders)
        {
            CoverAutoSnap testedCollider = collider.GetComponentInParent<CoverAutoSnap>();

            if (!testedCollider)
                continue; // No collider
            if (testedCollider == this)
                continue; // Current cover

            return true;
        }

        return false;
    }
}
