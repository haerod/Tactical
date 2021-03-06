using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Rules : MonoBehaviour
{
    public enum PassThrough {Everybody, Nobody, AlliesOnly}
    public PassThrough canPassThrough = PassThrough.Nobody;

    public enum SeeAnShotThroug { Everybody, Nobody, AlliesOnly}
    public SeeAnShotThroug canSeeAndShotThrough = SeeAnShotThroug.Everybody;

    public bool useDiagonals = false;

    public enum FirstCharacter {Random, ChoosenCharacter, FirstCharacterOfTheFirstTeam}
    public FirstCharacter firstCharacter = FirstCharacter.ChoosenCharacter;
    public C__Character choosenCharacter;

    public enum BotsPlayOrder { BeforePlayableCharacters, AfterPlayableCharacters}
    public BotsPlayOrder botsPlays = BotsPlayOrder.AfterPlayableCharacters;

    [Space]
    public int percentReductionByDistance = 5;

    [Space]
    public List<TeamInfos> teamInfos;

    public static M_Rules instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_Rules in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

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
