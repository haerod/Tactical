using System.Collections;
using UnityEngine;

public class GameCamera : MonoSingleton<GameCamera>
{
    public Transform target = null;

    [SerializeField] private float speed = .1f;

    [Header("REFERENCES")]

    [SerializeField] private Transform camTransform = null;

    [Header("SCREEN SHAKE")]

    [SerializeField] private float timeBetweenShakes = .2f;

    private Vector3 offset;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed); // oui c'est crade mais balek
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

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
