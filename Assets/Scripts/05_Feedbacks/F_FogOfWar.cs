using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;

public class F_FogOfWar : MonoBehaviour
{
    private List<Tile> viewArea;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Show visible elements of the fog of war.
    /// </summary>
    private void ShowVisibleElements(List<Tile> visibleTiles)
    {
        DisableViewLines();
        EnableViewLines(visibleTiles);
        DisplayCharacters(visibleTiles);
    }
    
    /// <summary>
    /// Shows and hides characters in fog of war.
    /// </summary>
    /// <param name="visibleTiles"></param>
    private void DisplayCharacters(List<Tile> visibleTiles)
    {
        List<C__Character> visibleCharacters = _characters.GetUnitsList()
            .Where(c =>
            {
                switch (_rules.visibleInFogOfWar)
                {
                    case M_Rules.VisibleInFogOfWar.Everybody:
                        return true;
                    case M_Rules.VisibleInFogOfWar.Allies:
                        return visibleTiles.Contains(c.tile) || c.team.IsAllyOf(_characters.current);
                    case M_Rules.VisibleInFogOfWar.InView:
                        return visibleTiles.Contains(c.tile);
                    default:
                        return false;
                }
            })
            .ToList();

        // Shows visible characters
        visibleCharacters
            .ForEach(c => c.anim.SetVisualActives(true));

        // Hides invisible characters
        _characters.GetUnitsList()
            .Except(visibleCharacters)
            .ToList()
            .ForEach(c => c.anim.SetVisualActives(false));
    }
    
    /// <summary>
    /// Enables lines on tiles around the fog zone.
    /// </summary>
    /// <param name="tilesInView"></param>
    private void EnableViewLines(List<Tile> tilesInView)
    {
        viewArea = tilesInView;

        foreach (Tile tile in tilesInView)
        {
            tile.SetFogMaskActive(true);

            // 4 tiles around if they are in tilesInView too
            List<Tile> aroundTiles = _board
                .GetTilesAround(tile, 1, false)
                .Intersect(tilesInView)
                .ToList();

            if (aroundTiles.Count == 4) continue; // CONTINUE : Is framed tile

            if (!tilesInView.Contains(_board.GetTileWithOffset(0, 1, tile)))
            {
                tile.EnableViewLine(Tile.Directions.Top);
            }
            if (!tilesInView.Contains(_board.GetTileWithOffset(0, -1, tile)))
            {
                tile.EnableViewLine(Tile.Directions.Down);
            }
            if (!tilesInView.Contains(_board.GetTileWithOffset(1, 0, tile)))
            {
                tile.EnableViewLine(Tile.Directions.Right);
            }
            if (!tilesInView.Contains(_board.GetTileWithOffset(-1, 0, tile)))
            {
                tile.EnableViewLine(Tile.Directions.Left);
            }
        }
    }

    /// <summary>
    /// Disables lines on tiles around the fog zone.
    /// </summary>
    private void DisableViewLines()
    {
        if (Utils.IsVoidList(viewArea)) 
            return;

        foreach (Tile t in viewArea)
        {
            t.SetFogMaskActive(false);
            t.DisableViewLine();
        }

        viewArea.Clear();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        ShowVisibleElements(startingCharacter.look.VisibleTiles());
        
        startingCharacter.move.OnUnitEnterTile += Move_OnUnitEnterTile;
        startingCharacter.move.OnMovementEnd += Move_OnMovementEnd;
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        endingCharacter.move.OnUnitEnterTile -= Move_OnUnitEnterTile;
        endingCharacter.move.OnMovementEnd -= Move_OnMovementEnd;
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        ShowVisibleElements(_characters.current.look.VisibleTiles());
    }
    
    private void Move_OnUnitEnterTile(object sender, Tile enteredTile)
    {
        ShowVisibleElements(_characters.current.look.VisibleTiles());
    }
}
