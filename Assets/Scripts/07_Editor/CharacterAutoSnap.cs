using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
public class CharacterAutoSnap : MonoBehaviour
{
    private C__Character current;
    private M_Board board;
    private M_Characters characters;
    private M_Rules rules;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Destroy(this);
            return;
        }
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        SetBaseParameters();
        SetParent();
        SetBaseCoordinates();
        AddToManager();
    }

    private void Update()
    {
        if (Application.isPlaying) return;
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        MoveCharacterAt(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        AutoSnap();
    }

    private void OnDestroy()
    {
        if (Application.isPlaying) return;
        if (PrefabStageUtility.GetCurrentPrefabStage() != null) return;

        RemoveFromManagersList();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// When two characters are at the same position, automatically reposition it at the closest free position.
    /// If isn't free position at 100 tiles of radius around, destroy it and return an error.
    /// </summary>
    private void AutoSnap()
    {
        if(!current.tile || IsAnotherCharacterOnTheTile() || !current.move.CanWalkOn(current.tile.type))
        {
            Tile newTile = GetClosestFreeTile();

            if(!newTile)
            {
                DestroyIfNoPlace();
                return; // EXIT : No tile to place character, remove it. 
            }

            MoveCharacterAt(newTile.x, newTile.y);
        }
    }

    /// <summary>
    /// Destroy the character if there is no place to drop it.
    /// Log an error.
    /// </summary>
    private void DestroyIfNoPlace()
    {
        Debug.LogError("No tile to place character. Destroy character.");
        DestroyImmediate(current.gameObject);
        RemoveFromManagersList();
    }

    /// <summary>
    /// Move character position at the asked coordinates.
    /// Rename the element.
    /// Set the new elements dirty.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void MoveCharacterAt(int x, int y)
    {
        current.move.x = Mathf.RoundToInt(x);
        current.move.y = Mathf.RoundToInt(y);
        current.transform.position = new Vector3(current.x, 0, current.y);

        current.infos.SetTeamMaterials();

        current.name = string.Format("{0} ({2}) - Team {1}",
            current.infos.designation,
            rules.teamInfos[current.infos.team].teamName,
            current.behavior.playable ? "PC" : "NPC");

        EditorUtility.SetDirty(current.gameObject); // Save the character modifications
        EditorUtility.SetDirty(current.move); // Save the character modifications
    }

    /// <summary>
    /// Return true if is another character is on the same tile. Else, return false.
    /// </summary>
    /// <returns></returns>
    private bool IsAnotherCharacterOnTheTile()
    {
        foreach (C__Character c in characters.characters)
        {
            if (c.tile == current.tile && c != current)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Return the closest free tile (walakble and unoccupied) at 100 tiles of distance.
    /// If there is no free tile, returns null.
    /// </summary>
    /// <returns></returns>
    private Tile GetClosestFreeTile()
    {
        Tile newTile = null;
        int distance = 1;

        while (newTile == null)
        {
            List<Vector2Int> aroundTiles = board.GetEmptySquareCoordinates(current.move.x, current.move.y, distance);

            foreach (Vector2Int coordinate in aroundTiles)
            {
                Tile checkedTile = board.GetTileAtCoordinates(coordinate.x, coordinate.y);

                if (!checkedTile) continue; // No tile
                if (checkedTile.IsOccupiedByCharacter()) continue; // Occupied
                if (!current.move.CanWalkOn(checkedTile.type)) continue; // Unwalakble

                newTile = checkedTile;
            }

            distance++;

            if (distance > 100)
            {
                foreach (Tile t in board.grid)
                {
                    if (!t) continue; // No tile
                    if (t.IsOccupiedByCharacter()) continue; // Occupied
                    if (!current.move.CanWalkOn(t.type)) continue; // Unwalakble

                    newTile = t;
                }

                return null; // EXIT : No free tile at 100 tiles of distance around - While breaker (security)
            }
        }

        return newTile;
    }

    /// <summary>
    /// Remove the object for its manager's list.
    /// </summary>
    private void RemoveFromManagersList()
    {
        if (characters) // Called on exit of the prefab stage
        {
            characters.characters.Remove(current);
            EditorUtility.SetDirty(characters);
        }
    }

    /// <summary>
    /// Add the element to its manager.
    /// </summary>
    private void AddToManager()
    {
        if (!characters.characters.Contains(current))
        {
            characters.characters.Add(current);
            EditorUtility.SetDirty(characters);
        }
    }

    /// <summary>
    /// Set the base coordinates of the element.
    /// </summary>
    private void SetBaseCoordinates()
    {
        Tile lastNewTile = board.grid.LastOrDefault();
        if(lastNewTile)
        {
            current.move.x = lastNewTile.x;
            current.move.y = lastNewTile.y;
        }
        else
        {
            DestroyIfNoPlace();
            return; // EXIT : No tile to place character, remove it
        }

        if (IsAnotherCharacterOnTheTile())
        {
            Tile newTile = GetClosestFreeTile();

            if (!newTile)
            {
                DestroyIfNoPlace();
                return; // EXIT : No tile to place character, remove it. 
            }

            current.move.x = newTile.x;
            current.move.y = newTile.y;
        }
    }

    /// <summary>
    /// Set the element parent.
    /// </summary>
    private void SetParent()
    {
        transform.parent = characters.transform;
    }

    /// <summary>
    /// Set the basic parameters.
    /// </summary>
    private void SetBaseParameters()
    {
        current = GetComponent<C__Character>();
        board = FindAnyObjectByType<M_Board>();
        characters = FindAnyObjectByType<M_Characters>();
        rules = FindAnyObjectByType<M_Rules>();
        EditorUtility.SetDirty(this);
    }
}
