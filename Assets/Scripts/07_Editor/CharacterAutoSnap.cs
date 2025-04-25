using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[ExecuteInEditMode]
public class CharacterAutoSnap : BaseAutoSnap
{
    [HideInInspector] public C__Character character; // Note : Let it serializable to be dirty.
    [HideInInspector] public M_Characters characters; // Note : Let it serializable to be dirty.
    [HideInInspector] public M_Rules rules; // Note : Let it serializable to be dirty.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR

    private void OnDestroy()
    {
        if (!IsInEditor())
            return; // Not in editor mode
        if (!characters)
            return; // Exit prefab mode

        characters.RemoveCharacter(character);
        rules.RemoveCharacter(character);
        EditorUtility.SetDirty(characters);
    }

#endif

    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override void SetParameters()
    {
        character = GetComponent<C__Character>();
        characters = FindAnyObjectByType<M_Characters>();
        rules = FindAnyObjectByType<M_Rules>();
        transform.parent = characters.transform;
    }
    protected override void MoveObject(Vector2Int coordinates)
    {
        character.MoveAt(coordinates.x, coordinates.y);
    }

    protected override void AddToManager()
    {
        rules.AddCharacter(character);
        characters.AddCharacter(character);
    }
    
    protected override void RemoveFromManager()
    {
        rules.RemoveCharacter(character);
        characters.RemoveCharacter(character);
    }
    protected override bool IsOnValidPosition()
    {
        Tile validTile = GetWalkableTileUnder();

        if (!validTile)
            return false;
        if (GetOtherCharacterOnTile(validTile))
            return false;

        return true;
    }
    protected override void SetParametersDirty()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(characters);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

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

        return colliders
            .Select(collider => collider.GetComponentInParent<C__Character>())
            .Where(testedCharacter => testedCharacter)
            .FirstOrDefault(testedCharacter => testedCharacter != character);
    }
}
