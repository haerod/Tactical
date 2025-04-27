using System.Collections.Generic;
using UnityEngine;
using static M__Managers;


public class F_CoversHolder : MonoBehaviour
{
    [SerializeField] private int coverFeedbackRadius = 3;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject coverFeedbackPrefab;

    private List<F_Covers> covers = new List<F_Covers>();
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        GenerateCoverFeedbacks();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void DisplayCoverFeedbacks(Vector2Int centralTileCoordinates)
    {
        List<Vector2Int> coordinatesWhereDisplay =
            _board.GetFullSquareCoordinatesWithEdge(centralTileCoordinates.x, centralTileCoordinates.y, coverFeedbackRadius);

        for (int i = 0; i < coordinatesWhereDisplay.Count; i++)
        {
            Vector3 worldCoordinates = new Vector3(
                coordinatesWhereDisplay[i].x,
                0,
                coordinatesWhereDisplay[i].y);
            
            covers[i].transform.position = worldCoordinates;
            covers[i].gameObject.SetActive(true);
        }
    }
    
    public void HideCoverFeedbacks() => covers.ForEach(c => c.gameObject.SetActive(false));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private void GenerateCoverFeedbacks()
    {
        for (int i = 0; i < Mathf.Pow(coverFeedbackRadius, 2); i++)
        {
            GameObject instaCoverFeedback = Instantiate(coverFeedbackPrefab, transform);
            instaCoverFeedback.SetActive(false);
            covers.Add(instaCoverFeedback.GetComponent<F_Covers>());
        }
    }
}
