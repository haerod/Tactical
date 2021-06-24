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
        //currentCharacter = characters.GetRandom();
        NewCurrentCharacter();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void ChangeCharacter()
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
        Camera.main.GetComponent<GameCamera>().target = currentCharacter.transform;
        _inputs.c = currentCharacter;
        _inputs.cValueChanged = true;
        _ui.SetActionPointText(currentCharacter.actionPoints.actionPoints.ToString(), currentCharacter);
        currentCharacter.move.EnableMoveArea();
        _ui.CheckFollowButton(); // Cheat
    }
}
