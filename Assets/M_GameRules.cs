using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_GameRules : MonoSingleton<M_GameRules>
{
    public enum PassAcross { Everybody, Nobody, AlliesOnly}
    public PassAcross canPassAcross = PassAcross.Nobody;

    public bool useDiagonals = false;

    public enum FirstCharacter { Random, CurrentCharacter, FirstOfHierarchy}
    public FirstCharacter firstCharacter = FirstCharacter.CurrentCharacter;

    [Space]
    public List<TeamInfos> teamInfos;

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

[System.Serializable]
public class TeamInfos
{
    public string teamName = "Name";
    public Material mat1;
    public Material mat2;
}
