using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameRules : MonoSingleton<M_GameRules>
{
    public enum PassAcross { Everybody, Nobody} // Allies // EnemiesAndAllies
    public PassAcross canPassAcross = PassAcross.Nobody;
    public bool useDiagonals = false;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
