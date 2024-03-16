using UnityEngine;
using System.Collections;

public class F_SelectionSquare : MonoBehaviour
{
    [SerializeField] private Transform squareTransform = null;
    [Range(.01f, .5f)]
    [SerializeField] private float squareOffset = .01f;
    [SerializeField] private Color inRangeColor = Color.white;
    [SerializeField] private Color outRangeColor = Color.grey;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enable and position the selection square.
    /// </summary>
    /// <param name="tile"></param>
    public void SetSquare(Vector3 position, bool inRange)
    {
        squareTransform.gameObject.SetActive(true);
        squareTransform.position = position + Vector3.up * squareOffset;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (inRange)
            sr.color = inRangeColor;
        else
            sr.color = outRangeColor;
    }

    /// <summary>
    /// Disable the selection square.
    /// </summary>
    public void DisableSquare()
    {
        squareTransform.gameObject.SetActive(false);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
