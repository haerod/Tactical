using System.Collections.Generic;
using System.Linq;
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

    public void DisplayCoverFeedbacks(Coordinates centralTileCoordinates, List<Coordinates> coversCoordinates, C__Character characterCovered)
    {
        List<Coordinates> coordinatesInSquare = _board
            .GetFullSquareCoordinatesWithRadius(centralTileCoordinates.x, centralTileCoordinates.y, coverFeedbackRadius);
        
        List<Coordinates> tilesWhereDisplay = coordinatesInSquare
            .Intersect(coversCoordinates)
            .ToList();
        
        for (int i = 0; i < coordinatesInSquare.Count; i++)
        {
            Coordinates currentCoordinates = coordinatesInSquare[i];
            
            Vector3 worldCoordinates = new Vector3(
                currentCoordinates.x,
                0,
                currentCoordinates.y);
            
            covers[i].transform.position = worldCoordinates;
            covers[i].gameObject.SetActive(tilesWhereDisplay.Contains(currentCoordinates));
        }
    }
    
    public void HideCoverFeedbacks() => covers.ForEach(c => c.gameObject.SetActive(false));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    private void GenerateCoverFeedbacks()
    {
        for (int i = 0; i < Mathf.Pow(coverFeedbackRadius*2+1, 2); i++)
        {
            GameObject newCoverFeedback = Instantiate(coverFeedbackPrefab, transform);
            newCoverFeedback.SetActive(false);
            covers.Add(newCoverFeedback.GetComponent<F_Covers>());
        }
    }
}
