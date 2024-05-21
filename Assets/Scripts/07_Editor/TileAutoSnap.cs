using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class TileAutoSnap : MonoBehaviour
{
    public Tile tile { get; private set; }

    [HideInInspector] public M_Board board;
    [HideInInspector] public bool isLocated; // Note : Let it public to be dirty.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (Application.isPlaying)
        {
            if (!isLocated)
                Destroy(gameObject);

            Destroy(this);    
            return; // Play mode
        }
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) 
            return; // Play mode

        tile = GetComponent<Tile>();
        board = FindAnyObjectByType<M_Board>();
        transform.parent = board.transform;         
        transform.hasChanged = true;
    }

    private void Update()
    {
        if (Application.isPlaying)
            return; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            return; // Prefab mode
        if (!transform.hasChanged)
            return; // Didnt move

        CheckGridPosition();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (isLocated)
            return; // Is located

        Gizmos.color = Color.red;
        Vector3 cubeSize = Vector3.one;
        cubeSize.y = .2f;
        Gizmos.DrawCube(transform.position, cubeSize);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying) 
            return; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) 
            return; // Prefab mode
        if (!board)
            return; // Exit prefab mode

        board.tileGrid.RemoveTile(tile);
        EditorUtility.SetDirty(board);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check if it collides a tile
    /// </summary>
    /// <returns></returns>
    private bool IsCollidingAnotherTile()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, Vector3.one * .4f);

        foreach (Collider collider in colliders)
        {
            Tile testedTile = collider.GetComponent<Tile>();

            if (!testedTile)
                continue; // No tile
            if (testedTile == tile)
                continue; // Same tile

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the tile is snapping somewhere
    /// </summary>
    private void CheckGridPosition()
    {
        Vector2Int coordinates = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.z));

        board.tileGrid.RemoveTile(tile);

        if (IsCollidingAnotherTile())
        {
            isLocated = false;
        }
        else
        {
            tile.Setup(coordinates);
            tile.MoveAtGridPosition(coordinates.x, coordinates.y);
            board.tileGrid.AddTile(tile);
            isLocated = true;
        }

        transform.hasChanged = false;

        EditorUtility.SetDirty(board);
    }
}