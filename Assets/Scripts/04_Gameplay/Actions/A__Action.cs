using UnityEngine;

[CreateAssetMenu(fileName = "New action", menuName = "Basic Unity Tactical Tool/Action", order = 5)]
public class A__Action : ScriptableObject
{
    [SerializeField] private A__ActionTrigger actionTrigger;
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the action trigger of this action.
    /// </summary>
    /// <returns></returns>
    public A__ActionTrigger GetActionTrigger() => actionTrigger;

    /// <summary>
    /// Returns true if the action trigger corresponds to the given trigger.
    /// </summary>
    /// <param name="trigger"></param>
    /// <returns></returns>
    public bool IsTriggered(A__ActionTrigger trigger) => actionTrigger == trigger;
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    
}
