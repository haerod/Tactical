using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static M__Managers;

public class Test : MonoBehaviour
{
    public static Test instance;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _units.current.inventory.items.Print();
            Debug.Log("here", gameObject);
        }
    }
}
