using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;
using Random = UnityEngine.Random;
using Cinemachine;

public class M_Camera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 4f;
    
    public static M_Camera instance => _instance ??= FindFirstObjectByType<M_Camera>();
    public static M_Camera _instance;
    
    private Transform target;
    private Camera camera;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!_instance)
            _instance = this;
        else
            Debug.LogError("There is more than one M_Camera in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }
    
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void Start()
    {
        _input.OnMovingCameraInput += Input_OnMovingCameraInput;
        _input.OnZoomingCameraInput += Input_OnZoomingCameraInput;
        _input.OnRecenterCameraInput += Input_OnRecenterCameraInput;
        _input.OnRotateRightInput += Input_OnRotateRightCameraInput;
        _input.OnRotateLeftInput += Input_OnRotateLeftCameraInput;
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the current camera.
    /// </summary>
    public Camera GetCurrentCamera() => camera ? camera : camera = Camera.main;
    
    /// <summary>
    /// Resets the camera's position to reach on its target (with the offset).
    /// </summary>
    public void ResetPosition()
    {
        cameraTarget.transform.position = new Vector3(
            target.transform.position.x,
            0,
            target.transform.position.z);
    }

    /// <summary>
    /// Rotates the camera, adding the given angle.
    /// </summary>
    /// <param name="angle"></param>
    public void RotateOnAngle(float angle)
    {
        cameraTarget.eulerAngles += new Vector3(0f, angle, 0f);
    }
    
    /// <summary>
    /// Camera shaking for more fun.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    /// <param name="timeBetweenShakes"></param>
    public void Shake(float duration = .02f, float intensity = .2f, float timeBetweenShakes = .02f) => 
        StartCoroutine(Shake_Co(duration, intensity, timeBetweenShakes));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Move the camera in the given direction.
    /// </summary>
    /// <param name="direction"></param>
    private void MoveInDirection(Vector2Int direction)
    {
        // Transform the input direction in world direction to follow the angle of the camera
        Vector3 moveDirection = cameraTarget.forward * direction.y + cameraTarget.right * direction.x;
        cameraTarget.position += moveDirection * moveSpeed * Time.deltaTime;
    }
    
    /// <summary>
    /// Zoom with the given amount.
    /// </summary>
    /// <param name="zoomAmount"></param>
    private void Zoom(float zoomAmount)
    {
        virtualCamera.m_Lens.OrthographicSize += zoomAmount;
        virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize, minZoom, maxZoom);
    }
    
    /// <summary>
    /// Sets the camera's target.
    /// </summary>
    /// <param name="newTarget"></param>
    private void SetTarget(Transform newTarget) => target = newTarget;

    /// <summary>
    /// Gets the center between multiple targets.
    /// </summary>
    /// <param name="targetsPositions"></param>
    /// <returns></returns>
    private Vector3 GetTargetsCenter(List<Vector3> targetsPositions)
    {
        if(targetsPositions.Count < 2)
            Debug.LogError("it needs to be at least 2 targets positions");
        
        Bounds targetsBounds = new Bounds(targetsPositions[0], Vector3.zero);
        
        foreach (Vector3 position in targetsPositions)
        {
            targetsBounds.Encapsulate(position);
        }
        return targetsBounds.center;
    }
    
    /// <summary>
    /// Coroutine of Shake() method. Only called by Shake().
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    /// <param name="timeBetweenShakes"></param>
    /// <returns></returns>
    private IEnumerator Shake_Co(float duration, float intensity, float timeBetweenShakes)
    {
        float shakeCurrentTime = 0;

        while(shakeCurrentTime < duration)
        {
            virtualCamera.transform.localPosition = Random.onUnitSphere * intensity;
            yield return new WaitForSeconds(timeBetweenShakes);
            shakeCurrentTime += timeBetweenShakes;
        }

        virtualCamera.transform.localPosition = Vector3.zero;
    }

    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Input_OnMovingCameraInput(object sender, Coordinates inputMoveDirection)
    {
        MoveInDirection(new Vector2Int(inputMoveDirection.x, inputMoveDirection.y));
    }

    private void Input_OnZoomingCameraInput(object sender, int zoomAmount)
    {
        Zoom(zoomAmount);
    }
    
    private void Input_OnRecenterCameraInput(object sender, EventArgs e)
    {
        ResetPosition();
    }

    private void Input_OnRotateLeftCameraInput(object sender, EventArgs e)
    {
        RotateOnAngle(90f);
    }

    private void Input_OnRotateRightCameraInput(object sender, EventArgs e)
    {
        RotateOnAngle(-90f);
    }

    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        _camera.SetTarget(startingCharacter.transform);
        _camera.ResetPosition();
    }

}