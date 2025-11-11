using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New color", menuName = "Basic Unity Tactical Tool/Color", order = 5)]
public class GameColor : ScriptableObject
{
    public Color color = Color.black;
}
