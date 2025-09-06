using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class C_UnitUI : MonoBehaviour
{
    [Header("REFERENCES")]
    
    [SerializeField] private UI_OrientToCamera orientToCamera;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Orients the in-world UI to the camera.
    /// </summary>
    public void OrientToCamera() => orientToCamera.OrientToCamera();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
