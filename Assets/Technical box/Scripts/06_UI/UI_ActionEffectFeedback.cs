using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static M__Managers;

public class UI_ActionEffectFeedback : MonoBehaviour
{
    [SerializeField] private float destructionDelay = 2;
    [SerializeField] private float textOffset = 2;
    [SerializeField] private float textMovementOffset = .25f;
    
    [Header("REFERENCES")]

    [SerializeField] private TextMeshProUGUI textValue;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private Camera cam;
    private Transform target;

    private float currentTime;
    private float fadeDelay;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, destructionDelay);
        fadeDelay = destructionDelay/2;
    }
    
    private void Update()
    {
        currentTime += Time.deltaTime;
        
        if(target)
            transform.position = cam.WorldToScreenPoint(target.position + (Vector3.up * (textOffset + textMovementOffset * currentTime/destructionDelay)));
        
        if (currentTime >= fadeDelay)
            canvasGroup.alpha = 1 - (currentTime-fadeDelay) / fadeDelay;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Positions the target on a new target.
    /// </summary>
    /// <param name="reference"></param>
    public void PositionAt(Transform reference) => target = reference;

    /// <summary>
    /// Writes the text on the action effect component.
    /// </summary>
    /// <param name="value"></param>
    public void SetText(string value) => textValue.text = value;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
}
