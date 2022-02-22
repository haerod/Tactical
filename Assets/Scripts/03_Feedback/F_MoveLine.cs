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
    /// Set the lines on the path, with the good colors.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="character"></param>
    /// <param name="endTile"></param>
    public void SetLines(List<Tile> path, C__Character character, Tile endTile)
    {
        int actionPoints = character.actionPoints.actionPoints;

        // Target tile is in/out action points' range
        if (path.Count - 1 > actionPoints) // Out
        {
            line.positionCount = actionPoints + 1;
            lineOut.gameObject.SetActive(true);
            lineOut.positionCount = path.Count - actionPoints;
            _feedback.actionCostText.SetActionCostText((path.Count - 1).ToString(), endTile.transform, true);
        }
        else // IN
        {
            lineOut.gameObject.SetActive(false);
            line.positionCount = path.Count;
            _feedback.actionCostText.SetActionCostText((path.Count - 1).ToString(), endTile.transform);
        }

        // Position line's points
        int i = 0;
        foreach (Tile t in path)
        {
            if (i <= actionPoints)
            {
                line.SetPosition(i, t.transform.position + Vector3.up * lineOffset);
            }
            else
            {
                if (i == actionPoints + 1)
                {
                    lineOut.SetPosition(i - (actionPoints + 1), path[i - 1].transform.position + Vector3.up * lineOffset);
                }
                lineOut.SetPosition(i - (actionPoints), t.transform.position + Vector3.up * lineOffset);
            }

            i++;
        }

        line.gameObject.SetActive(true);
    }

    /// <summary>
    /// Disable the lines.
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
