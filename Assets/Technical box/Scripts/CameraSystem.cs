using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Cinemachine;

/// <summary>
/// Class description
/// </summary>
public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public float moveSpeed = 25f;
    public float rotationSpeed = 200f;
    public float zoomSpeed = .25f;
    public float minZoom = 1.5f;
    public float maxZoom = 8f;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Update()
    {
        Move();
        Rotate();
        Zoom();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Move()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDirection.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDirection.x = 1f;

        if (Input.mousePosition.x <= 1) inputDirection.x = -1f;
        if (Input.mousePosition.y <= 1) inputDirection.z = -1f;
        if (Input.mousePosition.x >= Screen.width) inputDirection.x = +1f;
        if (Input.mousePosition.y >= Screen.height) inputDirection.z = +1f;
        
        // Transform the input direction in world direction to follow the angle of the camera
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void Rotate()
    {
        float rotationDirection = 0f;
        if (Input.GetKey(KeyCode.Q)) rotationDirection = +1f;
        if (Input.GetKey(KeyCode.E)) rotationDirection = -1f;
        
        transform.eulerAngles += new Vector3(0f, rotationDirection * rotationSpeed * Time.deltaTime, 0f);
    }
    
    private void Zoom()
    {
        if(Input.mouseScrollDelta.y > 0)
            virtualCamera.m_Lens.OrthographicSize -= zoomSpeed;
        if(Input.mouseScrollDelta.y < 0)
            virtualCamera.m_Lens.OrthographicSize += zoomSpeed;
        
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minZoom, maxZoom);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}
