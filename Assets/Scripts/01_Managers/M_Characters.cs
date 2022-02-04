using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static M__Managers;

public class M_Characters : MonoBehaviour
{
    public C__Character currentCharacter;
    
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

        switch (_rules.firstCharacter)
        {
            case M_Rules.FirstCharacter.Random:
                currentCharacter = characters.GetRandom();
                break;
            case M_Rules.FirstCharacter.CurrentCharacter:
                if(currentCharacter == null)
                {
                    Debug.LogError("current character is null in characters manager, set it", gameObject);
                }
                break;
            case M_Rules.FirstCharacter.FirstOfHierarchy:
                currentCharacter = characters[0];
                break;
            default:
                break;
        }

        NewCurrentCharacter();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Pass to the next player's turn
    /// </summary>
    public void NextTurn()
    {
        if (IsFinalTeam(currentCharacter))
        {
            Victory();
            return;
        }

        // Old character
        currentCharacter.ClearTilesFeedbacks();

        // New character
        currentCharacter = characters.Next(characters.IndexOf(currentCharacter));
        NewCurrentCharacter();
    }

    /// <summary>
    /// Pass to the playbale teammates turn.
    /// </summary>
    public void NextTeamCharacter()
    {
        // Old character
        currentCharacter.ClearTilesFeedbacks();

        // Find the playble teamates
        List<C__Character> team = characters
            .Where(o => o.Team() == currentCharacter.Team() && o.behavior.playable)
            .ToList();

        // EXIT : There is no playble teammates.
        if (team.Count <= 1) return;

        currentCharacter = team.Next(team.IndexOf(currentCharacter));
        NewCurrentCharacter();
    }

    /// <summary>
    /// Return true if the given character is the current character
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsCurrentCharacter(C__Character character)
    {
        if (character == currentCharacter) return true;
        else return false;
    }

    /// <summary>
    /// Do some things when a character is dead (ex: remove it from the playable character list)
    /// </summary>
    /// <param name="dead"></param>
    public void DeadCharacter(C__Character dead)
    {
        characters.Remove(dead);
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
    /// Enable victory screen and do the other things happening during victory
    /// </summary>
    public void Victory()
    {
        _ui.SetTurnPlayerUIActive(false);
        _ui.EnableEndScreen(currentCharacter);
        currentCharacter.ClearTilesFeedbacks();

        _inputs.SetClick(false);
        _inputs.ClearFeedbacksAndValues();
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
    }

    /// <summary>
    /// Do all the things happening when a new current character is designated (reset camera, clear visual feedbacks, update UI, etc.)
    /// </summary>
    private void NewCurrentCharacter()
    {
        // Inputs
        _inputs.ClearFeedbacksAndValues();

        // Camera
        _camera.target = currentCharacter.transform;
        _camera.ResetPosition();

        // Character
        currentCharacter.actionPoints.FullActionPoints();
        currentCharacter.ClearTilesFeedbacks();

        if(currentCharacter.behavior.playable) // Playable character (PC)
        {
            _inputs.SetClick();
            _ui.SetTurnPlayerUIActive(true);
            _ui.SetActionPointText(currentCharacter.actionPoints.actionPoints.ToString(), currentCharacter);
            currentCharacter.EnableTilesFeedbacks();
        }
        else // Non playable character (NPC)
        {
            _inputs.SetClick(false);
            _ui.SetTurnPlayerUIActive(false);
            currentCharacter.behavior.PlayBehavior();
        }

    }
}
