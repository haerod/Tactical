using System.Collections;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform target = null;
    [SerializeField] private float speed = .1f;

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

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

}
