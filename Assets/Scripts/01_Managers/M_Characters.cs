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
    [SerializeField] private List<Team> teamPlayOrder;
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
    /// Returns the full units list.
    /// </summary>
    /// <returns></returns>
    public List<C__Character> GetUnitsList() => characters;
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<C__Character> GetAlliesOf(C__Character unit) => characters
        .Where(testedUnit => testedUnit.team.IsAllyOf(unit))
        .ToList();
    
    /// <summary>
    /// Returns all enemies of the given unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<C__Character> GetEnemiesOf(C__Character unit) => characters
        .Where(testedUnit => testedUnit.team.IsEnemyOf(unit))
        .ToList();
    
    /// <summary>
    /// Adds a new unit in the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void AddUnit(C__Character unit)
    {
        GetUnitsList().Add(unit);
        
        if(!teamPlayOrder.Contains(unit.unitTeam))
            teamPlayOrder.Add(unit.unitTeam);
    }
    
    /// <summary>
    /// Removes a unit from the unit's list.
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(C__Character unit)
    {
        GetUnitsList().Remove(unit);
        
        if(IsAnotherUnitOfTheSameTeam(unit))
            return; // Is another unit of the same team
        
        teamPlayOrder.Remove(unit.unitTeam);
    }
    
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
    /// Returns true if the unit's team is the last team standing.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool IsFinalTeam(C__Character unit) => GetUnitsList()
            .Where(c => c != unit)
            .All(c => c.team.IsAllyOf(unit));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns true if is another unit in the given unit team. Else returns false.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool IsAnotherUnitOfTheSameTeam(C__Character unit) =>GetUnitsList()
            .Where(testedUnit => testedUnit != unit)
            .FirstOrDefault(testedUnit => testedUnit.team.IsAllyOf(unit));

    // ======================================================================
    // EVENTS
    // ======================================================================
}
