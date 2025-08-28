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
        A_Move.OnAnyMovementEnd += Move_OnAnyMovementEnd;
        A_Move.OnTileEnter += Move_OnTileEnter;
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
    private void ShowVisibleElements(List<Tile> visibleTiles) => SetFogVisualsActive(true, visibleTiles);
    
    /// <summary>
    /// Enables/disables the view lines on border tiles and enables/disables fog mask.
    /// </summary>
    private void SetFogVisualsActive(bool value, List<Tile> tilesInView = null)
    {
        if (!_rules.enableFogOfWar) 
            return; // No fog of war

        if (value)
        {
            EnableViewLines(tilesInView);
            DisplayCharactersInFogOfWar(tilesInView);
        }
        else
        {
            DisableViewLines();
        }
    }
    
    /// <summary>
    /// Shows and hides characters in fog of war.
    /// </summary>
    /// <param name="visibleTiles"></param>
    private void DisplayCharactersInFogOfWar(List<Tile> visibleTiles)
    {
        List<C__Character> visibleCharacters = _characters.GetCharacterList()
            .Where(c =>
            {
                if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Everybody)
                    return true;
                else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.Allies)
                    return visibleTiles.Contains(c.tile) || c.unitTeam == _characters.current.unitTeam;
                else if (_rules.visibleInFogOfWar == M_Rules.VisibleInFogOfWar.InView)
                    return visibleTiles.Contains(c.tile);
                else
                    Debug.LogError("No rule, please add one here.");

                return false;
            })
            .ToList();

        // Shows visible characters
        visibleCharacters
            .ForEach(c => c.anim.SetVisualActives(true));

        // Hides invisible characters
        _characters.GetCharacterList()
            .Except(visibleCharacters)
            .ToList()
            .ForEach(c => c.anim.SetVisualActives(false));
    }
    
    /// <summary>
    /// Enables view lines.
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
    /// Disables view lines.
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
        SetFogVisualsActive(false);
        ShowVisibleElements(startingCharacter.look.VisibleTiles());
    }

    private void Move_OnAnyMovementEnd(object sender, EventArgs e)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.behavior.playable)
            return;
        
        SetFogVisualsActive(false);
        ShowVisibleElements(currentCharacter.look.VisibleTiles());
    }
    
    private void Move_OnTileEnter(object sender, Tile e)
    {
        C__Character currentCharacter = _characters.current;
        
        if (!currentCharacter.behavior.playable)
            return;
        
        SetFogVisualsActive(false);
        ShowVisibleElements(currentCharacter.look.VisibleTiles());
    }
}
