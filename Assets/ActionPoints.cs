using UnityEngine;
using System.Collections;
using static M__Managers;

public class ActionPoints : MonoBehaviour
{
    public int actionPoints = 6;
    public int maxActionPoints = 6;

    [Header("REFERENCES")]

    [SerializeField] private Character c = null;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void RemoveActionPoints(int value = 1)
    {
        actionPoints -= value;
        _ui.SetActionPointText(actionPoints.ToString(), c);
    }

    public void AddActionPoints(int value = 1)
    {
        actionPoints += value;
        _ui.SetActionPointText(actionPoints.ToString(), c);
    }

    public void FullActionPoints()
    {
        actionPoints = maxActionPoints;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
