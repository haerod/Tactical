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
        // Old character
        currentCharacter.move.ClearAreaZone();

        // New character
        currentCharacter = characters.Next(characters.IndexOf(currentCharacter));
        NewCurrentCharacter();
    }

    public bool IsCurrentCharacter(Character character)
    {
        if (character == currentCharacter) return true;
        else return false;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void NewCurrentCharacter()
    {
        // Inputs
        _inputs.canClick = true;
        _inputs.ClearFeedbacks();
        _inputs.c = currentCharacter;
        _inputs.cValueChanged = true;

        // Camera
        Camera.main.GetComponent<GameCamera>().target = currentCharacter.transform;

        // UI
        _ui.SetActionPointText(currentCharacter.actionPoints.actionPoints.ToString(), currentCharacter);
        _ui.CheckFollowButton(); // Cheat

        // Character
        currentCharacter.actionPoints.FullActionPoints();

        if(currentCharacter.behaviour.playable) // Playable
        {
            currentCharacter.move.EnableMoveArea();
        }
        else // PNJ
        {
            _inputs.canClick = false;
            currentCharacter.behaviour.PlayBehaviour();
        }

    }
}
