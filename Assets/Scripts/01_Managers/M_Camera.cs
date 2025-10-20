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

    [SerializeField] private Camera currentCamera;
    
    public static M_Camera instance => _instance ??= FindFirstObjectByType<M_Camera>();
    public static M_Camera _instance;

    private float currentTime;
    private Vector3 positionToReach;
    private Vector3 startPosition;
    private Transform target;

    private float xMin, xMax, zMin, zMax;
    
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
        
        TileGrid boardTileGrid = _board.tileGrid;

        xMin = boardTileGrid.lowestX;
        xMax = boardTileGrid.higherX;
        zMin = boardTileGrid.lowestY;
        zMax = boardTileGrid.higherY;
    }

    private void OnDisable()
    {
        _input.OnMovingCameraInput -= Input_OnMovingCameraInput;
        _input.OnZoomingCameraInput -= Input_OnZoomingCameraInput;
        _input.OnRecenterCameraInput -= Input_OnRecenterCameraInput;
        _input.OnRotateRightInput -= Input_OnRotateRightCameraInput;
        _input.OnRotateLeftInput -= Input_OnRotateLeftCameraInput;
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
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
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget) => target = newTarget;
    
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
    public void Shake(float duration = .02f, float intensity = .2f, float timeBetweenShakes = .02f) => 
        StartCoroutine(Shake_Co(duration, intensity, timeBetweenShakes));

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Updates the camera position (clamped). Called by Update().
    /// </summary>
    private void UpdateCameraPosition()
    {
        if(!target)
            return; // No target
        
        currentTime += Time.deltaTime;

        Vector3 cameraPosition = Vector3.Lerp(
            startPosition,
            positionToReach,
            smoothMovingCurve.Evaluate(Mathf.Clamp01((currentTime * movingSpeedMultiplier) / smoothMovingTime)));

        transform.position = cameraPosition;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, xMin + xOffset, xMax + xOffset),
            target.transform.position.y + yOffset,
            Mathf.Clamp(transform.position.z, zMin + zOffset, zMax + +zOffset)
            );
    }

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
            currentCamera.transform.localPosition = Random.onUnitSphere * intensity;
            yield return new WaitForSeconds(timeBetweenShakes);
            shakeCurrentTime += timeBetweenShakes;
        }

        currentCamera.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Rotates the camera around Y axis.
    /// </summary>
    /// <param name="rotationValue"></param>
    private void RotateCamera(float rotationValue) => transform.Rotate(Vector3.up, rotationValue);

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
    
    private void Input_OnRecenterCameraInput(object sender, EventArgs e) => ResetPosition();
    
    private void Input_OnRotateLeftCameraInput(object sender, EventArgs e) => RotateCamera(90f);

    private void Input_OnRotateRightCameraInput(object sender, EventArgs e) => RotateCamera(-90f);
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingCharacter)
    {
        _camera.SetTarget(startingCharacter.transform);
        _camera.ResetPosition();
    }

}