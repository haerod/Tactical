using UnityEngine;
using System.Collections;

public class F_SelectionSquare : MonoBehaviour
{
    [SerializeField] private Transform squareTransform = null;
    [Range(.01f, .5f)]
    [SerializeField] private float squareOffset = .01f;

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
    public void SetSquare(Tile tile)
    {
        squareTransform.gameObject.SetActive(true);
        squareTransform.position = tile.transform.position + Vector3.up * squareOffset;
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
