using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class F_MoveLine : MonoBehaviour
{
    [Range(.01f, .5f)]
    [SerializeField] private float lineOffset = 0.05f;

    [Header("REFERENCES")]

    [SerializeField] private LineRenderer line = null;
    [SerializeField] private LineRenderer lineOut = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Sets the lines on the path, with the good colors.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="movementRange"></param>
    public void SetLines(List<Tile> path, int movementRange)
    {
        bool isEndTileInMovementRange = path.Count - 1 <= movementRange;

        //Set line in and out active
        line.gameObject.SetActive(true);
        lineOut.gameObject.SetActive(!isEndTileInMovementRange);

        if (isEndTileInMovementRange)
            line.positionCount = path.Count;
        else
        {
            line.positionCount = movementRange + 1;
            lineOut.positionCount = path.Count - movementRange;
        }

        // Position line's points
        int i = 0;
        path.ForEach(tile => {
            if (i <= movementRange)
                line.SetPosition(i, tile.transform.position + Vector3.up * lineOffset);
            else
            {
                if (i == movementRange + 1)
                {
                    lineOut.SetPosition(i - (movementRange + 1), path[i - 1].transform.position + Vector3.up * lineOffset);
                }
                lineOut.SetPosition(i - (movementRange), tile.transform.position + Vector3.up * lineOffset);
            }

            i++;
        });
    }

    /// <summary>
    /// Disables the lines.
    /// </summary>
    public void DisableLines()
    {
        line.gameObject.SetActive(false);
        lineOut.gameObject.SetActive(false);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
