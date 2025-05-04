using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using static M__Managers;

public class F_Covers : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private SpriteRenderer[] renderers;
    
    private Color coveredColour;
    private Color uncoveredColour;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        coveredColour = _feedback.GetCoveredColour();
        uncoveredColour = _feedback.GetUncoveredColour();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void DisplayAt(Coordinates centerCoordinates, CoverInfo info)
    {
        HideAllRenderers();
        
        if (!CanDisplay(centerCoordinates, info))
        {
            Hide();
            return; // No cover to display
        }
        
        MoveTo(info.GetCoverFeedbackCoordinates().ToVector3());
        ShowFeedbackRenderer(GetClosestFeedbackRenderer(info), info, centerCoordinates == info.GetCoveredCoordinates());;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        HideAllRenderers();
        gameObject.SetActive(false);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private SpriteRenderer GetClosestFeedbackRenderer(CoverInfo infos) => renderers
        .OrderBy(testedRenderer => Vector3.Distance(testedRenderer.transform.position, infos.GetCoveredCoordinates().ToVector3()))
        .First();

    private void HideAllRenderers() => renderers.ToList().ForEach(testedRenderer => testedRenderer.gameObject.SetActive(false));
    
    private bool CanDisplay(Coordinates centerCoordinates, CoverInfo info)
    {
        if(info.GetCoverFeedbackCoordinates() == null)
            return false ; // No covers around

        if (info.GetCoveredCoordinates() == centerCoordinates)
            return true; // Is the center tile (true)
        
        if (!info.GetIsCovered())
            return false; // Is uncovered position
        
        return true;
    }

    private void ShowFeedbackRenderer(SpriteRenderer feedbackRenderer, CoverInfo info, bool isCentralTile)
    {
        feedbackRenderer.sprite = _feedback.GetCoverFeedbackSprite(info.GetCoverType());
        feedbackRenderer.color = info.GetIsCovered() ? coveredColour : uncoveredColour;
        if(isCentralTile)
            feedbackRenderer.color = new Color(feedbackRenderer.color.r, feedbackRenderer.color.g, feedbackRenderer.color.b, 1);
        
        feedbackRenderer.gameObject.SetActive(true);
    }
    
    private void MoveTo(Vector3 destination) => transform.position = destination;
}

