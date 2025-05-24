using System;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test instance;
    
    public CoverInfo coverInfo;
    public List<CoverInfo> coverInfos;

    private void Awake()
    {
        instance = this;
    }
}
