using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_Actions : MonoBehaviour
{
    [SerializeField] private List<A__Action> actions;
    
    [Header("REFERENCES")]
    [SerializeField] private C__Character c;
    
    // ======================================================================
    // MONOBEHAVIOR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Character's actions subscribes to Input's events.
    /// </summary>
    public void SubscribeToEvents() => actions.ForEach(action => action.SubscribeToEvents());

    /// <summary>
    /// Character's actions unsubscribes to Input's events.
    /// </summary>
    public void UnsubscribeToEvents() => actions.ForEach(action => action.UnsubscribeToEvents());

    /// <summary>
    /// Returns true if the character has Heal action.
    /// </summary>
    /// <returns></returns>
    public bool HasHealAction() => actions.OfType<A_Heal>().Any();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}
