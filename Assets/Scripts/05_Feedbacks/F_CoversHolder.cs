using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;


public class F_CoversHolder : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject coverFeedbackPrefab;

    private List<F_Covers> coverFeedbacks;
    
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
    
    /// <summary>
    /// Displays all cover feedbacks around a central coordinates.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="coverInfos"></param>
    public void DisplayCoverFeedbacksAround(Coordinates centerCoordinates, List<CoverInfo> coverInfos)
    {
        for (int i = 0; i < coverFeedbacks.Count; i++)
        {
            if (i < coverInfos.Count)
                coverFeedbacks[i].DisplayAt(centerCoordinates, coverInfos[i]);
            else
                coverFeedbacks[i].Hide();
        }
    }
    
    /// <summary>
    /// Hide all the cover feedbacks.
    /// </summary>
    public void HideCoverFeedbacks() => coverFeedbacks
        .ForEach(c => c.gameObject.SetActive(false));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Instantiate the cover feedbacks before use it (pooling).
    /// </summary>
    private void GenerateCoverFeedbacks()
    {
        coverFeedbacks = new List<F_Covers>();
        int coverFeedbackRange = _feedback.GetCoverFeedbackRange();
        
        for (int i = 0; i < Mathf.Pow(coverFeedbackRange*2+1, 2); i++)
        {
            GameObject newCoverFeedback = Instantiate(coverFeedbackPrefab, transform);
            newCoverFeedback.SetActive(false);
            coverFeedbacks.Add(newCoverFeedback.GetComponent<F_Covers>());
        }
    }
}
