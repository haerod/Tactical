using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "New unit behavior", menuName = "Basic Unity Tactical Tool/Unit behavior", order = 5)]
public class UnitBehavior : ScriptableObject
{
    private enum TargetPriority {Closest, Furthest, LowestHealth, HighestHealth}
    [SerializeField] private TargetPriority targetPriority;
    
    [TextArea]
    [SerializeField] private string behaviorDescription;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    public override bool Equals(object other)
    {
        if (other is UnitBehavior type)
            return name == type.name;
        
        return false;
    }
    
    public override int GetHashCode() => base.GetHashCode();
    
    public static bool operator ==(UnitBehavior x, UnitBehavior y) => x.name == y.name;
    
    public static bool operator !=(UnitBehavior x, UnitBehavior y) => x.name != y.name;
    
    /// <summary>
    /// Returns the unit to target depending on the priority.
    /// </summary>
    /// <param name="askingUnit"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    public Unit GetPreferredTarget(Unit askingUnit, List<Unit> units)
    {
        if (units.Count == 0)
            return null; // No unit
        
        switch (targetPriority)
        {
            case TargetPriority.Closest:
                return units
                    .OrderBy(unit => Vector3.Distance(askingUnit.transform.position, unit.transform.position))
                    .FirstOrDefault(); // Closest unit
            case TargetPriority.Furthest:
                return units
                    .OrderByDescending(unit => Vector3.Distance(askingUnit.transform.position, unit.transform.position))
                    .FirstOrDefault(); // Furthest unit
            case TargetPriority.LowestHealth:
                return units
                    .OrderBy(unit => unit.health.currentHealth)
                    .FirstOrDefault(); // Unit with lowest health
            case TargetPriority.HighestHealth:
                return units
                    .OrderByDescending(unit => unit.health.currentHealth)
                    .FirstOrDefault(); // Unit with highest health
            default:
                return null;
        }
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}