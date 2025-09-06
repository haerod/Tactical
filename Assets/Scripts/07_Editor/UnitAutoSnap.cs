using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class UnitAutoSnap : BaseAutoSnap
{
    [HideInInspector] public U__Unit unit; // Note : Let it serializable to be dirty.
    [HideInInspector] public M_Units units; // Note : Let it serializable to be dirty.

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

#if UNITY_EDITOR

    private void OnDestroy()
    {
        if (!IsInEditor())
            return; // Not in editor mode
        if (!units)
            return; // Exit prefab mode

        units.RemoveUnit(unit);
        EditorUtility.SetDirty(units);
    }

#endif

    // ======================================================================
    // INHERITED
    // ======================================================================

    protected override void SetParameters()
    {
        unit = GetComponent<U__Unit>();
        units = FindAnyObjectByType<M_Units>();
        transform.parent = units.transform;
    }
    
    protected override void MoveObject(Coordinates coordinates)
    {
        unit.MoveAt(coordinates.x, coordinates.y);
    }

    protected override void AddToManager()
    {
        units.AddUnit(unit);
    }
    
    protected override void RemoveFromManager()
    {
        units.RemoveUnit(unit);
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
        EditorUtility.SetDirty(units);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Get the tile under the unit, if it can walk on.
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
            if (!unit.move.CanWalkOn(testedTile.type))
                continue; // Not walkable

            return testedTile;
        }

        return null;
    }

    /// <summary>
    /// Get the unit on the tile if it's not itself.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private U__Unit GetOtherCharacterOnTile(Tile tile)
    {
        Collider[] colliders = Physics.OverlapSphere(tile.transform.position, .1f);

        return colliders
            .Select(collider => collider.GetComponentInParent<U__Unit>())
            .Where(testedUnit => testedUnit)
            .FirstOrDefault(testedUnit => testedUnit != unit);
    }
}
