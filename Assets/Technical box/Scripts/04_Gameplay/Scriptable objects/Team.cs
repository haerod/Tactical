using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New team", menuName = "Basic Unity Tactical Tool/Team", order = 2)]
public class Team : ScriptableObject
{
    public Material mainMaterial;
    public Material secondaryMaterial;
}
