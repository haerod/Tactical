using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_OrientToCamera : MonoBehaviour
{
    private Camera cam;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        OrientToCamera();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Orient the object to the camera.
    /// </summary>
    public void OrientToCamera() => transform.forward = cam.transform.forward;

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
