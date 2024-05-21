using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class F_ViewLines : MonoBehaviour
{
    private List<Tile> viewArea;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enable view lines.
    /// </summary>
    /// <param name="tilesInView"></param>
    public void EnableViewLines(List<Tile> tilesInView)
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
    /// Disable view lines.
    /// </summary>
    public void DisableViewLines()
    {
        if (Utils.IsVoidList(viewArea)) return;

        foreach (Tile t in viewArea)
        {
        t.SetFogMaskActive(false);
            t.DisableViewLine();
        }

        viewArea.Clear();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}