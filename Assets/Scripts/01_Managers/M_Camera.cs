using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;
using Random = UnityEngine.Random;

public class M_Camera : MonoBehaviour
{
    [Header("MOVEMENT")]

    [SerializeField] private int movingSpeedMultiplier = 2;
    [Space]
    [SerializeField] private float smoothMovingTime = .3f;
    [SerializeField] private AnimationCurve smoothMovingCurve = null;

    [Header("CAMERA OFFSET")]

    public float xOffset = -3;
    public float yOffset = 5;
    public float zOffset = -3;

    [Header("ORTHOGRAPHIC ZOOM")]

    public float zoomMin = 1;
    public float zoomMax = 5;

    [Header("REFERENCES")]

    public static M_Camera instance;

    [SerializeField] private Camera currentCamera;

    private float currentTime;
    private Vector3 positionToReach;
    private Vector3 startPosition;
    private Transform target;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Camera in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }

    private void Start()
    {
        _input.OnMovingCameraInput += Input_OnMovingCameraInput;
        _input.OnZoomingCameraInput += Input_OnZoomingCameraInput;
        _input.OnRecenterCameraInput += Input_OnRecenterCameraInput;
    }
    
    private void Update()
    {
        if (currentTime < smoothMovingTime)
            UpdateCameraPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the current camera.
    /// </summary>
    public Camera GetCurrentCamera() => currentCamera;
    
    /// <summary>
    /// Sets the camera's target.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target) => this.target = target;
    
    /// <summary>
    /// Resets the camera's position to reach on its target (with the offset).
    /// </summary>
    public void ResetPosition()
    {
        positionToReach = new Vector3(
            target.position.x + xOffset,
            target.position.y + yOffset,
            target.position.z + zOffset);

        currentTime = 0;
        startPosition = transform.position;
    }

    /// <summary>
    /// Camera shaking for more fun.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    /// <param name="timeBetweenShakes"></param>
    public void Shake(float duration = .02f, float intensity = .2f, float timeBetweenShakes = .02f) => StartCoroutine(Shake_Co(duration, intensity, timeBetweenShakes));

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Updates the camera position (clamped). Called by Update().
    /// </summary>
    private void UpdateCameraPosition()
    {
        currentTime += Time.deltaTime;

        Vector3 cameraPosition = Vector3.Lerp(
            startPosition,
            positionToReach,
            smoothMovingCurve.Evaluate(Mathf.Clamp01((currentTime * movingSpeedMultiplier) / smoothMovingTime)));

        transform.position = cameraPosition;

        TileGrid boardTileGrid = _board.tileGrid;

        float xMin = boardTileGrid.lowestX;
        float xMax = boardTileGrid.higherX;
        float zMin = boardTileGrid.lowestY;
        float zMax = boardTileGrid.higherY;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, xMin + xOffset, xMax + xOffset),
            target.transform.position.y + yOffset,
            Mathf.Clamp(transform.position.z, zMin + zOffset, zMax + +zOffset)
            );
    }

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
        float currentTime = 0;

        while(currentTime < duration)
        {
            currentCamera.transform.localPosition = Random.onUnitSphere * intensity;
            yield return new WaitForSeconds(timeBetweenShakes);
            currentTime += timeBetweenShakes;
        }

        currentCamera.transform.localPosition = Vector3.zero;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Input_OnMovingCameraInput(object sender, Coordinates inputMoveDirection)
    {
        positionToReach = transform.position
            + transform.forward * inputMoveDirection.y 
            + transform.right * inputMoveDirection.x;
        startPosition = transform.position;
        currentTime = 0f;
    }

    private void Input_OnZoomingCameraInput(object sender, int zoomAmount)
    {
        currentCamera.orthographicSize += zoomAmount;
        currentCamera.orthographicSize = Mathf.Clamp(currentCamera.orthographicSize, zoomMin, zoomMax);
    }
    
    private void Input_OnRecenterCameraInput(object sender, EventArgs e) => _camera.ResetPosition();
}