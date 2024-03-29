﻿using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class Tile : MonoBehaviour
{
    [Header("COORDINATES (debug)")]

    public int x;
    public int y;

    [Header("PROPERTIES")]

    public TileType type;

    [Header("PATHFININDING VALUES (debug)")]
    public int cost;
    public int heuristic; // heuristic (10 = adjacent, 14 = diagonal)
    public int f; // f = cost + heuristic
    public Tile parent;

    [Header("MATERIALS (debug)")]
    [SerializeField] private Material basic = null;
    [SerializeField] private Material open = null;
    [SerializeField] private Material closed = null;
    [SerializeField] private Material path = null;
    [SerializeField] private Material area = null;
    [SerializeField] private Material attackable = null;
    [SerializeField] private Material range = null;

    [Header("REFERENCES")]

    [SerializeField] private MeshRenderer rend = null;
    [SerializeField] private MeshRenderer areaRend = null;
    [Space]
    [SerializeField] private TextMesh cText = null;
    [SerializeField] private TextMesh hText = null;
    [SerializeField] private TextMesh fText = null;
    [Space]
    [SerializeField] private GameObject areaObject = null;
    [SerializeField] private GameObject fogMask = null;
    [Space]
    [SerializeField] private GameObject topLine = null;
    [SerializeField] private GameObject downLine = null;
    [SerializeField] private GameObject leftLine = null;
    [SerializeField] private GameObject rightLine = null;

    public enum TileMaterial { Basic, Open, Closed, Path, Area, Attackable, Range} // Update the SetMaterial() method if add/remove a tile material.
    public enum Directions { Top, Down, Right, Left}

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Reset the pathfinding tiles value.
    /// </summary>
    public void ResetTileValues()
    {
        cost = 0;
        heuristic = 0;
        f = 0;
        parent = null;
    }

    /// <summary>
    /// Enable the view lines.
    /// </summary>
    public void EnableViewLine(Directions direction)
    {
        switch (direction)
        {
            case Directions.Top:
                topLine.SetActive(true);
                break;
            case Directions.Down:
                downLine.SetActive(true);
                break;
            case Directions.Right:
                rightLine.SetActive(true);
                break;
            case Directions.Left:
                leftLine.SetActive(true);
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// Disbale all the view lines.
    /// </summary>
    public void DisableViewLine()
    {
        topLine.SetActive(false);
        downLine.SetActive(false);
        rightLine.SetActive(false);
        leftLine.SetActive(false);
    }

    /// <summary>
    /// Enable or disable the fog mask
    /// </summary>
    /// <param name="value"></param>
    public void SetFogMaskActive(bool value)
    {
        fogMask.SetActive(value);
    }

    /// <summary>
    /// Set the appearance of a tile with the new material.
    /// </summary>
    /// <param name="mat"></param>
    public void SetMaterial(TileMaterial mat)
    {
        if(mat != TileMaterial.Area || mat != TileMaterial.Attackable)
        {
            areaObject.SetActive(false);
        }

        switch (mat)
        {
            case TileMaterial.Basic:
                rend.material = basic;
                break;
            case TileMaterial.Open:
                rend.material = open;
                break;
            case TileMaterial.Closed:
                rend.material = closed;
                break;
            case TileMaterial.Path:
                rend.material = path;
                break;
            case TileMaterial.Area:
                areaObject.SetActive(true);
                areaRend.material = area;
                break;
            case TileMaterial.Attackable:
                areaObject.SetActive(true);
                areaRend.material = attackable;
                break;
            case TileMaterial.Range:
                rend.material = range;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set the tile skin to "basic".
    /// Short cut of SetMaterial(TileMaterial.Basic).
    /// </summary>
    public void ResetTileSkin()
    {
        SetMaterial(TileMaterial.Basic);
    }

    /// <summary>
    /// Return true if the tile is occupied by a character.
    /// </summary>
    /// <returns></returns>
    public bool IsOccupiedByCharacter()
    {
        List<C__Character> characters;
        if (_characters) // M__Characters isn't initalized in editor
            characters = _characters.characters;
        else
            characters = FindAnyObjectByType<M_Characters>().characters;

        foreach (C__Character c in characters)
        {
            if (c.move.x == x && c.move.y == y)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Return the character on this tile.
    /// Return null if is nobody on this tile.
    /// </summary>
    /// <returns></returns>
    public C__Character Character()
    {
        foreach (C__Character c in _characters.characters)
        {
            if(c.tile == this) return c;
        }

        return null;
    }

    // DEBUG
    // =====

    /// <summary>
    /// Debug : Show the pathfinding values visually. 
    /// To show, enable the texts on the prefab, there are the TextMesh components in references, and call this method.
    /// </summary>
    public void ShowValues()
    {
        cText.gameObject.SetActive(true);
        hText.gameObject.SetActive(true);
        fText.gameObject.SetActive(true);

        cText.text = cost.ToString();
        hText.text = heuristic.ToString();
        fText.text = f.ToString();
    }

    /// <summary>
    /// Debug : Hide the pathfinding values visually.
    /// To hide, enable the texts on the prefab, there are the TextMesh components in references, and call this method.
    /// Uncomment this lines in M_TileBoard, in GenerateBoard() method for a better lisibility.
    /// </summary>
    public void HideValues()
    {
        cText.gameObject.SetActive(false);
        hText.gameObject.SetActive(false);
        fText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Enable/Disable the renderer.
    /// </summary>
    /// <param name="value"></param>
    public void SetRendererActive(bool value)
    {
        rend.enabled = value;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}