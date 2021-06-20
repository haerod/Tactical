using UnityEngine;
using System.Collections;

public class ActionPoints : MonoBehaviour
{
    public int actionPoints = 3;

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
        M_UI.instance.SetActionPointText(actionPoints.ToString(), c);
    }

    public void AddActionPoints(int value = 1)
    {
        actionPoints += value;
        M_UI.instance.SetActionPointText(actionPoints.ToString(), c);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
