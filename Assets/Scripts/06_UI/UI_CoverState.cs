using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class UI_CoverState : MonoBehaviour
{
    public Image coverStateImage;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void DisplayCoverState(CoverType coverType, Color color)
    {
        Color colorWithAlpha = new Color(color.r, color.g, color.b, 1);
        coverStateImage.color = colorWithAlpha;
        coverStateImage.sprite = coverType.GetCoverFeedbackSprite();
        coverStateImage.gameObject.SetActive(true);
    }
    
    public void HideCoverState() => coverStateImage.gameObject.SetActive(false);

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
