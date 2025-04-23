using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;

public class M_Characters : MonoBehaviour
{
    [Header("DEBUG")]
    public C__Character current;    
    [SerializeField] private List<C__Character> characters;
    
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
        OrderCharacterList();

        // Choose the first character
        switch (_rules.firstCharacter)
        {
            case M_Rules.FirstCharacter.Random:
                current = GetCharacterList().GetRandom();
                break;
            case M_Rules.FirstCharacter.ChosenCharacter:
                if(_rules.chosenCharacter == null)
                {
                    Debug.LogError("Chosen character is null in M_Rules, set it.", _rules.gameObject);
                }
                break;
            case M_Rules.FirstCharacter.FirstCharacterOfTheFirstTeam:
                current = GetCharacterList()[0];
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
    /// Remove the character of the character's list.
    /// </summary>
    /// <param name="deadCharacter"></param>
    public void RemoveDeadCharacter(C__Character deadCharacter)
    {
        RemoveCharacter(deadCharacter);
        OrderCharacterList();
    }

    /// <summary>
    /// Add a new character in the character's list.
    /// </summary>
    /// <param name="character"></param>
    public void AddCharacter(C__Character character) => GetCharacterList().Add(character);

    /// <summary>
    /// Remove a character from the character's list.
    /// </summary>
    /// <param name="character"></param>
    public void RemoveCharacter(C__Character character) => GetCharacterList().Remove(character);

    /// <summary>
    /// Returns the character's list.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> GetCharacterList() => characters;

    /// <summary>
    /// Do all the things happening when a new current character is designated (reset camera, clear visual feedbacks, update UI, etc.)
    /// </summary>
    public void NewCurrentCharacter(C__Character newCurrentCharacter)
    {
        // Old character
        if(current)
            current.HideTilesFeedbacks();

        // Inputs
        _input.ClearFeedbacksAndValues();

        // Change current character
        current = newCurrentCharacter;

        // Camera
        _camera.SetTarget(current.transform);
        _camera.ResetPosition();

        // Character
        current.HideTilesFeedbacks();

        // Playable character (PC)
        if (current.behavior.playable) 
        {
            _input.SetActiveClick();
            _ui.SetActivePlayerUI_Turn(true);
        }
        // Non playable character (NPC)
        else
        {
            _input.SetActiveClick(false);
            _ui.SetActivePlayerUI_Turn(false);
            current.behavior.PlayBehavior();
        }

        current.EnableTilesFeedbacks();
    }

    /// <summary>
    /// Return true if the character's team is the last team standing.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsFinalTeam(C__Character character)
    {
        foreach (C__Character c in GetCharacterList())
        {
            if (c == character) continue;
            if (c.Team() != character.Team()) return false;
        }

        return true;
    }

    /// <summary>
    /// Return the team members.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="excludeCharacter"> if true, exclude the character of the list. </param>
    /// <param name="excludeNPC"> if true, exclude the NPCs of the list. </param>
    /// <param name="excludeCharactersWhoHavePlayed"> if true, exclude characters of the list without action points. </param>
    /// <returns></returns>
    public List<C__Character> GetTeam(C__Character character, bool excludeCharacter, bool excludeNPC, bool excludeCharactersWhoHavePlayed)
    {
        List<C__Character> team = GetCharacterList()
            .Where(o => o.Team() == character.Team())
            .ToList();

        if (excludeCharacter)
            team.Remove(character);

        if (excludeNPC)
            team = team
                .Where(o => o.behavior.playable)
                .ToList();
        
        if (excludeCharactersWhoHavePlayed)
            team = team
                .Where(o => o.CanPlay())
                .ToList();

        return team;
    }

    /// <summary>
    /// Return the team playable characters.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="excludeCharacter"></param>
    /// <returns></returns>
    public List<C__Character> GetTeamPC(C__Character character, bool excludeCharacter = false)
    {
        List<C__Character> team = GetCharacterList()
            .Where(o => o.Team() == character.Team())
            .Where(o => o.behavior.playable)
            .ToList();

        if (excludeCharacter)
            team.Remove(character);

        return team;
    }

    /// <summary>
    /// Return the team non playable characters.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="excludeCharacter"></param>
    /// <returns></returns>
    public List<C__Character> GetTeamNPC(C__Character character, bool excludeCharacter = false)
    {
        List<C__Character> team = GetCharacterList()
            .Where(o => o.Team() == character.Team())
            .Where(o => !o.behavior.playable)
            .ToList();

        if (excludeCharacter)
            team.Remove(character);

        return team;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Order the character list by team, then by PC/NPC (depending on the Rules).
    /// </summary>
    private void OrderCharacterList()
    {
        switch (_rules.botsPlay)
        {
            case M_Rules.BotsPlayOrder.BeforePlayableCharacters:
                characters = GetCharacterList()
                    .OrderBy(o => o.team.name)
                    .ThenBy(o => o.behavior.playable)
                    .ToList();
                break;

            case M_Rules.BotsPlayOrder.AfterPlayableCharacters:
                characters = GetCharacterList()
                    .OrderBy(o => o.team.name)
                    .ThenByDescending(o => o.behavior.playable)
                    .ToList();
                break;
            default:
                break;
        }
    }
}
