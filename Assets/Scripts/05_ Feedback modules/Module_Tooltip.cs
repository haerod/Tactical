using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Displays tooltips on screen.
/// </summary>
public class Module_Tooltip : MonoBehaviour
{
    [SerializeField] private float displayDelay = .5f;
    
    [Header("REFERENCES")]
    
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject tooltipTextBoxPrefab;
    [SerializeField] private Transform layoutGroup;
    [SerializeField] private RectTransform tooltipRect;
    
    private Coroutine timerCoroutine;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        GameEvents.OnAnyTooltipHovered += GameEvents_OnOnAnyTooltipHovered;
        GameEvents.OnAnyTooltipExit += GameEvents_OnOnAnyTooltipExit;
    }
    
    private void Update()
    {
        if(!panel.activeInHierarchy)
            return; // Not active

        PositionTooltip();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Moves the tooltip to mouse position, depending on the position on screen.
    /// </summary>
    private void PositionTooltip()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 newPivot = new (
            Mathf.Round(mousePos.x / Screen.width), 
            Mathf.Round(mousePos.y / Screen.height));
        
        tooltipRect.pivot = newPivot;
        tooltipRect.position = mousePos;
    }

    /// <summary>
    /// Displays a tooltip part by given tooltip in the list.
    /// </summary>
    /// <param name="tooltips"></param>
    private void ShowTooltip(List<string> tooltips)
    {
        foreach (Transform child in layoutGroup)
        {
            Destroy(child.gameObject);
        }
        
        foreach (string tooltip in tooltips)
        {
            GameObject newTooltip = Instantiate(tooltipTextBoxPrefab, layoutGroup);
            newTooltip.GetComponentInChildren<TextMeshProUGUI>().text = tooltip;
        }
        
        panel.SetActive(true);
    }

    /// <summary>
    /// Hides the tooltip.
    /// </summary>
    private void HideTooltip() => panel.SetActive(false);

    private IEnumerator DelayedSpawn(List<string> tooltips)
    {
        yield return new WaitForSeconds(displayDelay);
        
        if(tooltips.Count > 0)
            ShowTooltip(tooltips);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void GameEvents_OnOnAnyTooltipHovered(object sender, List<string> content)
    {
        if(timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        timerCoroutine = StartCoroutine(DelayedSpawn(content));
    }
    
    private void GameEvents_OnOnAnyTooltipExit(object sender, EventArgs e)
    {
        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
        
        HideTooltip();
    }
}