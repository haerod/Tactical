using System.Collections;
using UnityEngine;

public class GameCamera : MonoSingleton<GameCamera>
{

    [Header("MOVE")]

    public float xOffset = -3;
    public float yOffset = 5;
    public float zOffset = -3;

    [Space]

    public float movingTime = .5f;
    public AnimationCurve movingTimeCurve = null;

    [Header("SCREEN SHAKE")]

    public float timeBetweenShakes = .2f;

    [Header("REFERENCES")]

    public Transform camTransform = null;

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