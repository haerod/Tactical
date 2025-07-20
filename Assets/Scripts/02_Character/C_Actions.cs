using System.Collections.Generic;
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

    public void SubscribeToEvents() => actions.ForEach(action => action.SubscribeToEvents());
    public void UnsubscribeToEvents() => actions.ForEach(action => action.UnsubscribeToEvents());

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    // ======================================================================
    // EVENTS
    // ======================================================================

}
