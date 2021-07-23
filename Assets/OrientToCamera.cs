﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrientToCamera : MonoBehaviour
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
        Orient();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void Orient()
    {
        transform.forward = cam.transform.forward;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}