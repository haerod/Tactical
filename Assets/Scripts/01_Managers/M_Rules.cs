using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Rules : MonoBehaviour
{
    [Header("MOVEMENT")]
    
    public bool useDiagonals = false;

    public enum PassThrough {Everybody, Nobody, AlliesOnly}
    public PassThrough canPassThrough = PassThrough.Nobody;

    [Header("VISION")]

    public int percentReductionByDistance = 5;

    public enum SeeAnShotThroug { Everybody, Nobody, AlliesOnly}
    public SeeAnShotThroug canSeeAndShotThrough = SeeAnShotThroug.Everybody;

    [Header("TURNS")]

    public C__Character chosenCharacter;
    public enum FirstCharacter {Random, ChosenCharacter, FirstCharacterOfTheFirstTeam}
    public FirstCharacter firstCharacter = FirstCharacter.ChosenCharacter;

    public enum BotsPlayOrder { BeforePlayableCharacters, AfterPlayableCharacters}
    public BotsPlayOrder botsPlay = BotsPlayOrder.AfterPlayableCharacters;

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