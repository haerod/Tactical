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
    [SerializeField] GameColor coveredColor;
    [SerializeField] GameColor uncoveredColor;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Displays the feedback on a renderer, depending on the given info.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="info"></param>
    public void DisplayAt(Coordinates centerCoordinates, CoverInfo info)
    {
        HideAllRenderers();

        if (!CanDisplay(centerCoordinates, info))
        {
            Hide();
            return; // No cover to display
        }
        
        MoveTo(info.GetCoverFeedbackCoordinates().ToVector3());
        ShowFeedbackRenderer(GetClosestFeedbackRenderer(info), info, centerCoordinates == info.GetCoveredCoordinates());
    }

    /// <summary>
    /// Displays the feedback, depending on the given info.
    /// </summary>
    /// <param name="info"></param>
    public void Display(CoverInfo info)
    {
        HideAllRenderers();
        
        if(info == null)
            return; // No info
        
        MoveTo(info.GetCoverFeedbackCoordinates().ToVector3());
        ShowFeedbackRenderer(GetClosestFeedbackRenderer(info), info, true);;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disables the game object.
    /// </summary>
    public void Hide() => HideAllRenderers();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Returns the closest feedback renderer of the position.
    /// </summary>
    /// <param name="infos"></param>
    /// <returns></returns>
    private SpriteRenderer GetClosestFeedbackRenderer(CoverInfo infos) => renderers
        .OrderBy(testedRenderer => Vector3.Distance(testedRenderer.transform.position, infos.GetCoveredCoordinates().ToVector3()))
        .First();

    /// <summary>
    /// Hides all renderers.
    /// </summary>
    private void HideAllRenderers() => renderers.ToList().ForEach(testedRenderer => testedRenderer.gameObject.SetActive(false));
    
    /// <summary>
    /// Returns true if the feedback needs to be displayed.
    /// </summary>
    /// <param name="centerCoordinates"></param>
    /// <param name="info"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Enables the given feedback renderer and choose its appearance.
    /// </summary>
    /// <param name="feedbackRenderer"></param>
    /// <param name="info"></param>
    /// <param name="isCentralTile"></param>
    private void ShowFeedbackRenderer(SpriteRenderer feedbackRenderer, CoverInfo info, bool isCentralTile)
    {
        feedbackRenderer.sprite = info.GetCoverType().GetCoverFeedbackSprite();
        feedbackRenderer.color = info.GetIsCovered() ? coveredColor.color : uncoveredColor.color;
        if(isCentralTile)
            feedbackRenderer.color = new Color(feedbackRenderer.color.r, feedbackRenderer.color.g, feedbackRenderer.color.b, 1);
        
        feedbackRenderer.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Moves the feedbacks to a given destination.
    /// </summary>
    /// <param name="destination"></param>
    private void MoveTo(Vector3 destination) => transform.position = destination;
}

