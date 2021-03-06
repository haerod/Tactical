using UnityEngine;
using System.Collections;
using static M__Managers;

public class C_ActionPoints : MonoBehaviour
{
    public int actionPoints = 6;
    public int maxActionPoints = 6;

    [Header("REFERENCES")]

    [SerializeField] private C__Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Remove "value" action points (basically one).
    /// </summary>
    /// <param name="value"></param>
    public void RemoveActionPoints(int value = 1)
    {
        actionPoints -= value;
        _ui.SetActionPointText(actionPoints.ToString(), c);
    }

    /// <summary>
    /// Add "avalue action points (basically one).
    /// </summary>
    /// <param name="value"></param>
    public void AddActionPoints(int value = 1)
    {
        actionPoints += value;
        _ui.SetActionPointText(actionPoints.ToString(), c);
    }

    /// <summary>
    /// Set current action points value to max action points value.
    /// </summary>
    public void FullActionPoints()
    {
        actionPoints = maxActionPoints;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
