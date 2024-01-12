using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static M__Managers;

public class M_Characters : MonoBehaviour
{
    [HideInInspector] public C__Character current;    
    [HideInInspector] public List<C__Character> characters;
    public static M_Characters instance;

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
            Debug.LogError("There is more than one M_Characters in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    private void Start()
    {
        FillCharacterList();

        // Choose the first character
        switch (_rules.firstCharacter)
        {
            case M_Rules.FirstCharacter.Random:
                current = characters.GetRandom();
                break;
            case M_Rules.FirstCharacter.ChoosenCharacter:
                if(_rules.choosenCharacter == null)
                {
                    Debug.LogError("Choosen character is null in M_Rules, set it", _rules.gameObject);
                }
                break;
            case M_Rules.FirstCharacter.FirstCharacterOfTheFirstTeam:
                current = characters[0];
                break;
            default:
                break;
        }

        NewCurrentCharacter(current);
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Add a new character in the character's list.
    /// </summary>
    /// <param name="newChara"></param>
    public void AddNewCharacter(C__Character newChara)
    {
        characters.Add(newChara);
        OrderCharacterList();
    }

    /// <summary>
    /// Remove the character of the character's list.
    /// </summary>
    /// <param name="dead"></param>
    public void RemoveDeadCharacter(C__Character dead)
    {
        characters.Remove(dead);
        OrderCharacterList();
    }

    /// <summary>
    /// Do all the things happening when a new current character is designated (reset camera, clear visual feedbacks, update UI, etc.)
    /// </summary>
    public void NewCurrentCharacter(C__Character newCurrentCharacter)
    {
        // Old character
        if(current)
            current.ClearTilesFeedbacks();

        // Inputs
        _input.ClearFeedbacksAndValues();

        // Check if it's a new team's turn
        if (newCurrentCharacter.team != current.team)
            _turns.NewTeamTurn(GetTeam(newCurrentCharacter, false, false, false));

        // Change current character
        current = newCurrentCharacter;

        // Camera
        _camera.target = current.transform;
        _camera.ResetPosition();

        // Character
        current.ClearTilesFeedbacks();

        if(current.behavior.playable) // Playable character (PC)
        {
            _input.SetActiveClick();
            _ui.SetActivePlayerUI_Turn(true);
            _ui.SetActionPointText(current.actionPoints.movementRange.ToString(), current);
            current.EnableTilesFeedbacks();
        }
        else // Non playable character (NPC)
        {
            _input.SetActiveClick(false);
            _ui.SetActivePlayerUI_Turn(false);
            current.behavior.PlayBehavior();
            current.EnableTilesFeedbacks(false);
        }
    }

    /// <summary>
    /// Return true if the character's team is the last team standing.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsFinalTeam(C__Character character)
    {
        foreach (C__Character c in characters)
        {
            if (c != character && c.Team() != character.Team()) return false;
        }

        return true;
    }

    /// <summary>
    /// Return the team members.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="excludeCharacter"> if true, exclude the character of the list. </param>
    /// <param name="excludeNPC"> if true, exclude the NPCs of the list. </param>
    /// <param name="excludeEmptyActionPoints"> if true, exclude characters of the list without action points. </param>
    /// <returns></returns>
    public List<C__Character> GetTeam(C__Character character, bool excludeCharacter, bool excludeNPC, bool excludeEmptyActionPoints)
    {
        List<C__Character> team = characters
            .Where(o => o.Team() == character.Team())
            .ToList();

        if (excludeCharacter)
            team.Remove(character);

        if (excludeNPC)
            team = team
                .Where(o => o.behavior.playable)
                .ToList();
        
        if (excludeEmptyActionPoints)
            team = team
                .Where(o => o.actionPoints.movementRange > 0)
                .ToList();

        return team;
    }


    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Search and all characters and add them in the "characters" list
    /// </summary>
    private void FillCharacterList()
    {
        characters = FindObjectsOfType<C__Character>().ToList();
        OrderCharacterList();
    }

    /// <summary>
    /// Order the character list by team, then by PC/NPC (depending the Rules).
    /// </summary>
    private void OrderCharacterList()
    {
        switch (_rules.botsPlays)
        {
            case M_Rules.BotsPlayOrder.BeforePlayableCharacters:
                characters = characters
                    .OrderBy(o => o.Team())
                    .ThenBy(o => o.behavior.playable)
                    .ToList();
                break;

            case M_Rules.BotsPlayOrder.AfterPlayableCharacters:
                characters = characters
                    .OrderBy(o => o.Team())
                    .ThenByDescending(o => o.behavior.playable)
                    .ToList();
                break;
            default:
                break;
        }
    }
}
