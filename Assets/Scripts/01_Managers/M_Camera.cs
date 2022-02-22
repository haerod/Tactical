using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class M_Camera : MonoBehaviour
{
    [Header("MOVE")]

    public float xOffset = -3;
    public float yOffset = 5;
    public float zOffset = -3;

    [Space]

    public float movingTime = .3f;
    public AnimationCurve movingTimeCurve = null;

    [Header("REFERENCES")]

    public Transform camTransform = null;
    public static M_Camera instance;

    private float currentTime;
    private Vector3 positionToReach;
    private Vector3 startPosition;
    public Transform target;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_Camera in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Update()
    {
        UpdateCameraPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Move the camera in the given direction.
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {
        positionToReach = transform.position + direction;
        currentTime = 0f;
        startPosition = transform.position;
    }

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

        Vector3 cameraPoz = Vector3.Lerp(
            startPosition,
            positionToReach,
            movingTimeCurve.Evaluate(Mathf.Clamp01(currentTime / movingTime)));

        transform.position = cameraPoz;

        float xMin = _board.grid.OrderBy(o => o.x).FirstOrDefault().transform.position.x;
        float xMax = _board.grid.OrderByDescending(o => o.x).FirstOrDefault().transform.position.x;
        float zMin = _board.grid.OrderBy(o => o.y).FirstOrDefault().transform.position.z;
        float zMax = _board.grid.OrderByDescending(o => o.y).FirstOrDefault().transform.position.z;

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
    IEnumerator Shake_Co(float duration, float intensity, float timeBetweenShakes)
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
}