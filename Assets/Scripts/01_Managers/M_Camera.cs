using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Camera : MonoBehaviour
{
    [Header("OFFSET")]

    public float xOffset = -3;
    public float yOffset = 5;
    public float zOffset = -3;

    [Header("SPEED")]

    [SerializeField] private int movingSpeedMultiplier = 2;
    [Space]
    public float smoothMovingTime = .3f;
    public AnimationCurve smoothMovingCurve = null;

    [Header("REFERENCES")]

    public Transform camTransform = null;
    public static M_Camera instance;

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
    }

    private void Update()
    {
        if (currentTime < smoothMovingTime)
            UpdateCameraPosition();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) 
            return; // Editor is in edit mode.

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(positionToReach, .5f);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Set the camera's target.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target) => this.target = target;

    /// <summary>
    /// Reset the camera's position to reach on its target (with the offset).
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
    /// Camera shake for more fun.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="intensity"></param>
    /// <param name="timeBetweenShakes"></param>
    public void Shake(float duration = .02f, float intensity = .2f, float timeBetweenShakes = .02f)
    {
        StartCoroutine(Shake_Co(duration, intensity, timeBetweenShakes));
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Update the camera position (clamped). Called by Update().
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
            camTransform.localPosition = Random.onUnitSphere * intensity;
            yield return new WaitForSeconds(timeBetweenShakes);
            currentTime += timeBetweenShakes;
        }

        camTransform.localPosition = Vector3.zero;
    }

    // ======================================================================
    // EVENTS
    // ======================================================================

    private void Input_OnMovingCameraInput(object sender, Vector2Int inputMoveDirection)
    {
        positionToReach = transform.position
            + transform.forward * inputMoveDirection.y 
            + transform.right * inputMoveDirection.x;
        startPosition = transform.position;
        currentTime = 0f;
    }
}