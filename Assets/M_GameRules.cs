using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameRules : MonoBehaviour
{
    public static M_GameRules inst;

    public enum PassAcross { Everybody, Nobody} // Allies // EnemiesAndAllies
    public PassAcross canPassAcross = PassAcross.Nobody;
    public bool useDiagonals = false;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (!inst)
        {
            inst = this;
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
