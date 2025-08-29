using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static M__Managers;


public class F_CoversHolder : MonoBehaviour
{
    [SerializeField] private int coverFeedbackRange = 2;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject coverFeedbackPrefab;

    private List<F_Covers> coverFeedbacks;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        GenerateCoverFeedbacks();
        InputEvents.OnFreeTile += InputEvents_OnFreeTile;
        InputEvents.OnOccupiedTile += InputEvents_OnOccupiedTile;
        A_Move.OnAnyMovementStart += Move_OnAnyMovementStart;
        _input.OnNoTile += Input_OnNoTile;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Displays all cover feedbacks around a central coordinates.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="coverInfos"></param>
    private void DisplayCoverFeedbacksAround(Coordinates centerCoordinates, List<CoverInfo> coverInfos)
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
    /// Displays the cover infos of a character pointed by the current character.
    /// </summary>
    /// <param name="coverInfo"></param>
    private void DisplayTargetCoverFeedback(CoverInfo coverInfo)
    {
        HideCoverFeedbacks();
        coverFeedbacks[0].Display(coverInfo);
    }
    
    /// <summary>
    /// Hides all the cover feedbacks.
    /// </summary>
    private void HideCoverFeedbacks() => coverFeedbacks
        .ForEach(c => c.Hide());
    
    /// <summary>
    /// Instantiates the cover feedbacks before use it (pooling).
    /// </summary>
    private void GenerateCoverFeedbacks()
    {
        coverFeedbacks = new List<F_Covers>();
        
        for (int i = 0; i < Mathf.Pow(coverFeedbackRange*2+1, 2); i++)
        {
            F_Covers newCoverFeedback = Instantiate(coverFeedbackPrefab, transform).GetComponent<F_Covers>();
            coverFeedbacks.Add(newCoverFeedback);
            newCoverFeedback.Hide();
        }
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnFreeTile(object sender, Tile freeTile)
    {
        DisplayCoverFeedbacksAround(
            freeTile.coordinates, 
            _characters.current.cover.GetAllCoverInfosInRangeAt(freeTile.coordinates, coverFeedbackRange));
    }
    
    private void InputEvents_OnOccupiedTile(object sender, Tile occupiedTile)
    {
        DisplayTargetCoverFeedback(occupiedTile.character.cover.GetCoverStateFrom(_characters.current));
    }
    
    private void Input_OnNoTile(object sender, EventArgs e)
    {
        HideCoverFeedbacks();
    }
    
    private void Move_OnAnyMovementStart(object sender, EventArgs e)
    {
        HideCoverFeedbacks();
    }

}
