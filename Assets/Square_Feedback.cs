using UnityEngine;
using System.Collections;

public class Square_Feedback : MonoBehaviour
{
    [SerializeField] private Transform squareTransform = null;
    [Range(.01f, .5f)]
    [SerializeField] private float squareOffset = .01f;

    public void SetSquare(Tile tile)
    {
        squareTransform.gameObject.SetActive(true);
        squareTransform.position = tile.transform.position + Vector3.up * squareOffset;
    }

    public void DisableSquare()
    {
        squareTransform.gameObject.SetActive(false);
    }
}
