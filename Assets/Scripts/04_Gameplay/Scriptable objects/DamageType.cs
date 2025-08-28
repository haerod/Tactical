using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New damage type", menuName = "Basic Unity Tactical Tool/Damage type", order = 4)]
public class DamageType : ScriptableObject
{
    public override bool Equals(object other)
    {
        if (other is DamageType type)
            return name == type.name;
        
        return false;
    }

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(DamageType x, DamageType y) => x.name == y.name;

    public static bool operator !=(DamageType x, DamageType y) => x.name != y.name;
}
