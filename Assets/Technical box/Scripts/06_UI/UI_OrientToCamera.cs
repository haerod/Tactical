using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class UI_OrientToCamera : MonoBehaviour
{
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

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
    public void OrientToCamera()
    {
        if(_camera.currentCamera)
            transform.forward = _camera.currentCamera.transform.forward;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
