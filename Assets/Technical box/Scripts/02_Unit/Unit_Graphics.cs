using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class description
/// </summary>
public class Unit_Graphics : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _torso;
    [SerializeField] private Transform _belly;
    [SerializeField] private Transform _leftArm;
    [SerializeField] private Transform _rightArm;
    [SerializeField] private Transform _leftLeg;
    [SerializeField] private Transform _rightLeg;
    [SerializeField] private Transform _leftFoot;
    [SerializeField] private Transform _rightFoot;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
    public Transform head => _head;
    public Transform torso => _torso;
    public Transform belly => _belly;
    public Transform leftArm => _leftArm;
    public Transform rightArm => _rightArm;
    public Transform leftLeg => _leftLeg;
    public Transform rightLeg => _rightLeg;
    public Transform leftFoot => _leftFoot;
    public Transform rightFoot => _rightFoot;
    public Transform leftHand => _leftHand;
    public Transform rightHand => _rightHand;
    
    [Space]
    
    [SerializeField] private Transform _rootBone;
    public Transform rootBone => _rootBone;
    
    [Space]
    
    [SerializeField] private Vector3 _handPositionOffset;
    [SerializeField] private Vector3 _handRotationOffset;
    public Vector3 handPositionOffset => _handPositionOffset;
    public Vector3 handRotationOffset => _handRotationOffset;
    
    [Space]
    
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderers;
    [SerializeField] private List<GameObject> _visuals;
    public List<SkinnedMeshRenderer> skinnedMeshRenderers => _skinnedMeshRenderers;
    public List<GameObject> visuals => _visuals;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enables or disables visuals of the unit.
    /// </summary>
    public void SetVisualActives(bool value) => visuals.ForEach(o => o.SetActive(value));
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
}