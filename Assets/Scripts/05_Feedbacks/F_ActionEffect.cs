using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class F_ActionEffect : MonoBehaviour
{
    [SerializeField] private float destructionDelay = 2;
    [SerializeField] private float textOffset = 2;

    [Header("REFERENCES")]

    [SerializeField] private Text textValue = null;
    [SerializeField] private Transform panelTransform = null;

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
        panelTransform.position = cam.WorldToScreenPoint(target.position + (Vector3.up * textOffset));
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Position the target on a new target.
    /// </summary>
    /// <param name="reference"></param>
    public void PositionAt(Transform reference) => target = reference;

    /// <summary>
    /// Write the text on the action effect component.
    /// </summary>
    /// <param name="value"></param>
    public void SetText(string value) => textValue.text = value;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
