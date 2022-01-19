using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionEffect_Feedback : MonoBehaviour
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
        Destroy(gameObject, destructionDelay);
        cam = Camera.main;
    }

    private void Update()
    {
        panelTransform.position = cam.WorldToScreenPoint(target.position + (Vector3.up * textOffset));
        //panelTransform.position += ;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void PositionAt(Transform reference)
    {
        target = reference;
    }

    public void SetText(string value)
    {
        textValue.text = value;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
