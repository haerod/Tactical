using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Characters : MonoBehaviour
{
    public static M_Characters instance;

    public List<Character> characters;
    public Character currentCharacter;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

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
        M_PlayerInputs.inst.c = currentCharacter;
        M_PlayerInputs.inst.cValueChanged = true;
        M_UI.instance.SetActionPointText(currentCharacter.actionPoints.actionPoints.ToString(), currentCharacter);
    }
}
