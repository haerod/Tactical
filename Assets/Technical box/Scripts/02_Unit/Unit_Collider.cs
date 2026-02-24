using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils;

/// <summary>
/// Class description
/// </summary>
public class Unit_Collider : MonoBehaviour
{
    [SerializeField] private float _standHeight = 1.8f;
    [SerializeField] private float _standYCenter = 0.9f;
    
    [Space]
    
    [SerializeField] private float _crouchHeight = 1f;
    [SerializeField] private float _crouchYCenter = .5f;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private Unit _unit;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _unit.move.OnMovementStart += Move_OnMovementStart;
        _unit.move.OnMovementEnd += Move_OnMovementEnd;
        
        if(_unit.cover.AreCoversAround())
            Crouch();
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Setups the collider on crouch.
    /// </summary>
    private void Crouch()
    {
        _capsuleCollider.height = _crouchHeight;
        _capsuleCollider.center = new Vector3(_capsuleCollider.center.x,_crouchYCenter,_capsuleCollider.center.z);
    }
    
    /// <summary>
    /// Setups the collider on stand.
    /// </summary>
    private void Stand()
    {
        _capsuleCollider.height = _standHeight;
        _capsuleCollider.center = new Vector3(_capsuleCollider.center.x,_standYCenter,_capsuleCollider.center.z);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Move_OnMovementStart(object sender, EventArgs e)
    {
        Stand();
    }
    
    private void Move_OnMovementEnd(object sender, EventArgs e)
    {
        if(_unit.cover.AreCoversAround())
            Crouch();
    }
}