using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New weapon", menuName = "Basic Unity Tactical Tool/Weapon", order = 3)]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Vector2Int damagesRange = new Vector2Int(3, 5);
    public List<DamageType> damageType;
}
