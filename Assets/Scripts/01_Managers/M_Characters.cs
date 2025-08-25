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

    public event EventHandler<C__Character> OnCharacterHover;
    public event EventHandler<C__Character> OnCharacterExit;
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
        _input.OnTileEnter += InputOnTileEnter;
        _input.OnTileExit += InputOnTileExit;
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
    /// Do all the things happening when a new current character is designated (reset camera, clear visual feedbacks, update UI, etc.)
    /// </summary>
    public void NewCurrentCharacter(C__Character newCurrentCharacter)
    {
        OnCharacterTurnEnd?.Invoke(this, current);
        
        // Old character
        if (current)
        {
            current.actions.UnsubscribeToEvents();
            current.unitUI.Hide();
        }

        // Change current character
        current = newCurrentCharacter;

        // Camera
        _camera.SetTarget(current.transform);
        _camera.ResetPosition();

        // Character
        current.HideTilesFeedbacks();
        current.unitUI.Display();

        // Playable character (PC)
        if (current.behavior.playable) 
        {
            current.actions.SubscribeToEvents();
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
        
        OnCharacterTurnStart?.Invoke(this, newCurrentCharacter);
    }

    /// <summary>
    /// Return true if the character's team is the last team standing.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsFinalTeam(C__Character character) => 
        GetCharacterList()
            .Where(c => c != character)
            .All(c => c.Team() == character.Team());

    /// <summary>
    /// Return the team members of a character.
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public List<C__Character> GetTeamMembers(C__Character character) => 
        GetCharacterList()
            .Where(c => c.Team() == character.Team())
            .ToList();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputOnTileEnter(object sender, Tile tile)
    {
        if(tile.character)
            OnCharacterHover?.Invoke(this, tile.character);
    }

    private void InputOnTileExit(object sender, Tile tile)
    {
        if(tile.character)
            OnCharacterExit?.Invoke(this, tile.character);
    }
}
