using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class CharacterAutoSnap : MonoBehaviour
{
    [HideInInspector] public C__Character character;
    [HideInInspector] public M_Characters characters;
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
            return; // Prefab mode

        character = GetComponent<C__Character>();
        characters = FindAnyObjectByType<M_Characters>();
        transform.parent = characters.transform; 
        transform.hasChanged = true;
    }

    private void OnDrawGizmos()
    {
        if (isLocated)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.up * -.5f, Vector3.one);
    }

    private void Update()
    {
        if (Application.isPlaying)
            return; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            return; // Prefab stage

        CheckGridPosition();

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying) 
            return; // Play mode
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) 
            return; // Prefab mode
        if (!characters)
            return; // Exit prefab mode

        characters.characters.Remove(character);
        EditorUtility.SetDirty(characters);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Check if the character is snapping somewhere.
    /// </summary>
    private void CheckGridPosition()
    {
        isLocated = false;
        Tile allowedTile = GetWalkableTileUnder();

        characters.characters.Remove(character);

        if (!allowedTile)
            return; // No allowed tile
        if (GetOtherCharacterOnTile(allowedTile))
            return; // Occupied

        isLocated = true;
        characters.characters.Add(character);

        if (!transform.hasChanged)
            return; // Didn't move

        character.MoveAt(allowedTile.x, allowedTile.y);
        transform.hasChanged = false;

        EditorUtility.SetDirty(characters);
    }

    /// <summary>
    /// Get the tile under the character, if it can walk on.
    /// </summary>
    /// <returns></returns>
    private Tile GetWalkableTileUnder()
    {
        Collider[] colliderTileArray = Physics.OverlapSphere(transform.position, .1f);

        foreach (Collider colliderTile in colliderTileArray)
        {
            Tile testedTile = colliderTile.GetComponent<Tile>();

            if (!testedTile)
                continue; // No tile
            if (!character.move.CanWalkOn(testedTile.type))
                continue; // Not walkable

            return testedTile;
        }

        return null;
    }

    /// <summary>
    /// Get the character on the tile if it's not itself.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private C__Character GetOtherCharacterOnTile(Tile tile)
    {
        Collider[] colliders = Physics.OverlapSphere(tile.transform.position, .1f);

        foreach (Collider collider in colliders)
        {
            C__Character testedCharacter = collider.GetComponentInParent<C__Character>();

            if (!testedCharacter)
                continue; // No character

            if (testedCharacter == character)
                continue; // Current character

            return testedCharacter;
        }

        return null;
    }

}
