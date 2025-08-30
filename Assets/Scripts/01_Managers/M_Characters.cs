using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static M__Managers;

public class M_Characters : MonoBehaviour
{
    [HideInInspector] public C__Character current;    
    [SerializeField] private List<C__Character> characters;

    public event EventHandler<C__Character> OnCharacterTurnStart;
    public event EventHandler<C__Character> OnCharacterTurnEnd;
    
    public static M_Characters instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError("There is more than one M_Characters in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }

    private void Start()
    {
        NewCurrentCharacter(_rules.GetFirstCharacter());
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Returns the character's list.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> GetCharacterList() => characters;

    /// <summary>
    /// Adds a new character in the character's list.
    /// </summary>
    /// <param name="character"></param>
    public void AddCharacter(C__Character character) => GetCharacterList().Add(character);

    /// <summary>
    /// Removes a character from the character's list.
    /// </summary>
    /// <param name="character"></param>
    public void RemoveCharacter(C__Character character) => GetCharacterList().Remove(character);
    
    /// <summary>
    /// Does all the things happening when a new current character is designated (reset camera, clear visual feedbacks, update UI, etc.)
    /// </summary>
    public void NewCurrentCharacter(C__Character newCurrentCharacter)
    {
        if(current)
            OnCharacterTurnEnd?.Invoke(this, current);

        current = newCurrentCharacter;
        
        OnCharacterTurnStart?.Invoke(this, newCurrentCharacter);
    }

    /// <summary>
    /// Returns true if the character's team is the last team standing.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsFinalTeam(C__Character character) => 
        GetCharacterList()
            .Where(c => c != character)
            .All(c => c.team.IsAllyOf(character));

    /// <summary>
    /// Returns the team members of a character.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public List<C__Character> GetTeamMembers(C__Character character) => 
        GetCharacterList()
            .Where(c => c.team.IsAllyOf(character))
            .ToList();
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
}
