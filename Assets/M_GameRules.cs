using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameRules : MonoSingleton<M_GameRules>
{
    public enum PassAcross { Everybody, Nobody, AlliesOnly}
    public PassAcross canPassAcross = PassAcross.Nobody;

    public bool useDiagonals = false;

    public enum FirstCharacter { Random, CurrentCharacter, FirstOfList}
    public FirstCharacter firstCharacter = FirstCharacter.CurrentCharacter;

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
