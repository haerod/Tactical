﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static M__Managers;
using System;

public class C_Look : MonoBehaviour
{
    [SerializeField] private int range = 5;
    [SerializeField] private List<TileType> visualObstacles = null;

    [Header("REFERENCES")]
    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enable the view tiles.
    /// </summary>
    public void EnableViewTiles()
    {
        List<Tile> aroundTiles = _pathfinding.AroundTiles(c.tile, range, true);

        // Find all tiles in view
        List<Tile> toEnable =
            _pathfinding.AroundTiles(c.tile, range, true)
            .Where(o => HasSightOn(o))
            .Where(o => !visualObstacles.Contains(o.type))
            .ToList();

        _feedback.SetViewLinesActive(true, toEnable);
    }

    /// <summary>
    /// Return true if the character has the tile on sight.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool HasSightOn(Tile tile)
    {
        List<Tile> los = LineOfSight(tile);

        if (AreObstaclesOn(los))
            return false; // Exit : obstacles
        if (los.Count + 1 > range)
            return false; // Exit : out of range

        return true; // Exit : has sight on target
    }

    /// <summary>
    /// Return the line of sight of the character.
    /// </summary>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    public List<Tile> LineOfSight(Tile targetTile)
    {
        return _pathfinding.LineOfSight(c.tile, targetTile).ToList();
    }

    /// <summary>
    /// Return the closest enemy on sight.
    /// </summary>
    /// <returns></returns>
    public C__Character ClosestEnemyOnSight()
    {
        return _characters.characters
            .Where(o => o != c) // remove emitter
            .Where(o => o.Team() != c.Team()) // remove allies
            .Where(o => HasSightOn(o.tile)) // get all enemies on sight
            .OrderBy(o => LineOfSight(o.tile).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Return true if the character has obstacles on its line of sight.
    /// </summary>
    /// <param name="lineOfSight"></param>
    /// <returns></returns>
    private bool AreObstaclesOn(List<Tile> lineOfSight)
    {
        if (Utils.IsVoidList(lineOfSight)) return false;

        foreach (Tile t in lineOfSight)
        {
            if (t == null) continue; // Next : it's a hole.
            if (visualObstacles.Contains(t.type)) return true; // EXIT : Big obstacle.

            // Character
            C__Character chara = t.Character();
            if (!chara) continue; // Next : there is no character.

            if (_rules.canSeeAndShotThrough == M_Rules.SeeAnShotThroug.Nobody)
            {
                if (!chara.health.IsDead()) return true; // EXIT : Other character.
            }
            else if (_rules.canSeeAndShotThrough == M_Rules.SeeAnShotThroug.AlliesOnly)
            {
                // Are obstacle if enemy && alive
                if ((chara.infos.team != c.infos.team) && (!chara.health.IsDead())) return true; // EXIT : Enemy
            }
        }

        return false; // EXIT : No obstacle.
    }
}
