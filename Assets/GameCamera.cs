using System.Collections;
using UnityEngine;

public class GameCamera : MonoSingleton<GameCamera>
{

    [Header("MOVE")]

    [SerializeField] private float xOffset = -3;
    [SerializeField] private float yOffset = 5;
    [SerializeField] private float zOffset = -3;

    [Space]

    [SerializeField] private float movingTime = .5f;
    [SerializeField] private AnimationCurve movingTimeCurve = null;
    
    [Header("ZOOM")]

    [SerializeField] private float percentZoomMin = .5f;
    [SerializeField] private float distanceTick = .15f;
    [SerializeField] private float currentDistancePercent = .9f;

    [Header("REFERENCES")]

    [SerializeField] private Transform camTransform = null;

    [Header("SCREEN SHAKE")]

    [SerializeField] private float timeBetweenShakes = .2f;

    private float currentTime;
    private Vector3 positionToReach;
    private Vector3 startPosition;
    [HideInInspector] public Transform target;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    void Update()
    {
        currentTime += Time.deltaTime;

        transform.position = Vector3.Lerp(
            startPosition, 
            positionToReach, 
            movingTimeCurve.Evaluate(Mathf.Clamp01(currentTime/movingTime)));
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ResetPosition()
    {
        positionToReach = new Vector3(
            target.position.x + xOffset,
            target.position.y + yOffset,
            target.position.z + zOffset);

        currentTime = 0;
        startPosition = transform.position;
    }

    public void Move(Vector3 direction)
    {
        positionToReach = transform.position + direction;
        //positionToReach = Vector3.Lerp(target.position, positionToReach, 1);
        //print(positionToReach);

        currentTime = 0f;
        startPosition = transform.position;
    }

    public void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            currentDistancePercent = Mathf.Clamp(currentDistancePercent - distanceTick, percentZoomMin, 1);
            //StartMoving();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            currentDistancePercent = Mathf.Clamp(currentDistancePercent + distanceTick, percentZoomMin, 1);
            //StartMoving();
        }
    }

    public void Shake(float duration = .02f, float intensity = .2f)
    {
        StartCoroutine(Shake_Co(duration, intensity));
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    IEnumerator Shake_Co(float duration, float intensity)
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

/*

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    void Update()
    {
        if (MouseManager.instance.mouseMoves)
        {
            StartMoving();
        }
        Zoom();
        Follow();
    }

    // Called by OnMouseMoving (event, on MouseManager)
    // Or called by Zoom
    public void StartMoving()
    {
        Vector2 pointerOffset = MouseManager.instance.DistanceWithCharacter();
        pointerOffset = new Vector2(
            Mathf.Clamp(pointerOffset.x, maxDistance * -1, maxDistance) * offsetMultiplierX,
            Mathf.Clamp(pointerOffset.y, maxDistance * -1, maxDistance)) * offsetMultiplierZ;

        positionToReach = new Vector3(
            target.position.x + xOffset + pointerOffset.x,
            target.position.y + yOffset,
            target.position.z + zOffset + pointerOffset.y);

        positionToReach = Vector3.Lerp(target.position, positionToReach, currentDistancePercent);

        currentTime = 0f;
        isMoving = true;
        startPosition = transform.position;
    }

    private void Follow()
    {
        if (isMoving)
        {
            currentTime += Time.deltaTime;

            if (currentTime < movingTime)
            {
                transform.position = Vector3.Lerp(startPosition, positionToReach, movingTimeCurve.Evaluate((currentTime / movingTime)));
            }
            else
            {
                StopMoving();
            }
        }
    }

    private void Zoom()
    {
        if (Hinput.mouse.scroll.up) // forward
        {
            currentDistancePercent = Mathf.Clamp(currentDistancePercent - distanceTick, percentZoomMin, 1);
            StartMoving();
        }
        else if (Hinput.mouse.scroll.down) // backwards
        {
            currentDistancePercent = Mathf.Clamp(currentDistancePercent + distanceTick, percentZoomMin, 1);
            StartMoving();
        }
    }

    private void StopMoving()
    {
        isMoving = true;
    }
}

 
*/
