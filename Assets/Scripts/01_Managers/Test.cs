using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test instance;
    
    public C__Character testCharacter;
    public List<Tile> visibleTiles;
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
            print(testCharacter.look.visibleTiles.Count);
    }
}
