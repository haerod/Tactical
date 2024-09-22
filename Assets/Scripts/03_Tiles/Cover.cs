using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Return true if it's beteween two asked tiles.
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    public bool IsBetweenTiles(Tile tile1, Tile tile2)
    {
        if (tile1.x == tile2.x)
        {
            if ((tile1.y + tile2.y) / 2f == transform.position.z)
                return true;
        }

        if (tile1.y == tile2.y)
            if ((tile1.x + tile2.x) / 2f == transform.position.x)
                return true;

        return false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}