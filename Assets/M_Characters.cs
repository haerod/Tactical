using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class M_Characters : MonoSingleton<M_Characters>
{
    public List<Character> characters;
    public Character currentCharacter;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        switch (_rules.firstCharacter)
        {
            case M_GameRules.FirstCharacter.Random:
                currentCharacter = characters.GetRandom();
                break;
            case M_GameRules.FirstCharacter.CurrentCharacter:
                if(currentCharacter == null)
                {
                    Debug.LogError("current character is null in characters manager, set it", gameObject);
                }
                break;
            case M_GameRules.FirstCharacter.FirstOfList:
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

    public void NextTurn()
    {
        if (IsVictory())
        {
            print("victory");
            _inputs.SetClick(false);
            _inputs.ClearFeedbacksAndValues();
            currentCharacter.ClearTilesFeedbacks();
            return;
        }

        // Old character
        currentCharacter.ClearTilesFeedbacks();

        // New character
        currentCharacter = characters.Next(characters.IndexOf(currentCharacter));
        NewCurrentCharacter();
    }

    public bool IsCurrentCharacter(Character character)
    {
        if (character == currentCharacter) return true;
        else return false;
    }

    public void DeadCharacter(Character dead)
    {
        characters.Remove(dead);
    }

    // Is the last character // NEXT : team
    public bool IsFinalCharacter(Character character)
    {
        foreach (Character c in characters)
        {
            if (c != character) return false;
        }

        return true;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void NewCurrentCharacter()
    {
        // Inputs
        _inputs.ClearFeedbacksAndValues();

        // Camera
        Camera.main.GetComponentInParent<GameCamera>().target = currentCharacter.transform;

        // UI
        _ui.SetActionPointText(currentCharacter.actionPoints.actionPoints.ToString(), currentCharacter);
        _ui.CheckFollowButton(); // Cheat

        // Character
        currentCharacter.actionPoints.FullActionPoints();
        currentCharacter.ClearTilesFeedbacks();

        if(currentCharacter.behaviour.playable) // PC
        {
            _inputs.SetClick();
            _ui.SetPlayerUIActive(true);
            currentCharacter.EnableTilesFeedbacks();
        }
        else // NPC
        {
            _inputs.SetClick(false);
            _ui.SetPlayerUIActive(false);
            currentCharacter.behaviour.PlayBehaviour();
        }

    }

    private bool IsVictory()
    {
        return (characters.Count <= 1);
    }
}
