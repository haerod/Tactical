using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static M__Managers;

public class UI_ActionEffectFeedback : MonoBehaviour
{
    [SerializeField] private float destructionDelay = 2;
    [SerializeField] private float textOffset = 2;

    [Header("REFERENCES")]

    [SerializeField] private TextMeshProUGUI textValue;

    private Camera cam;
    private Transform target;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, destructionDelay);
    }

    private void Update()
    {
        transform.position = cam.WorldToScreenPoint(target.position + (Vector3.up * textOffset));
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
