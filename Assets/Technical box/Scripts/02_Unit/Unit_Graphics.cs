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
    public Transform head => _head;
    [SerializeField] private Transform _torso;
    public Transform torso => _torso;
    [SerializeField] private Transform _belly;
    public Transform belly => _belly;
    [SerializeField] private Transform _leftArm;
    public Transform leftArm => _leftArm;
    [SerializeField] private Transform _rightArm;
    public Transform rightArm => _rightArm;
    [SerializeField] private Transform _leftLeg;
    public Transform leftLeg => _leftLeg;
    [SerializeField] private Transform _rightLeg;
    public Transform rightLeg => _rightLeg;
    [SerializeField] private Transform _leftFoot;
    public Transform leftFoot => _leftFoot;
    [SerializeField] private Transform _rightFoot;
    public Transform rightFoot => _rightFoot;
    [SerializeField] private Transform _leftHand;
    public Transform leftHand => _leftHand;
    [SerializeField] private Transform _rightHand;
    public Transform rightHand => _rightHand;
    [Space]
    [SerializeField] private Transform _rootBone;
    public Transform rootBone => _rootBone;
    [Space]
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderers;
    public List<SkinnedMeshRenderer> skinnedMeshRenderers => _skinnedMeshRenderers;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    // ======================================================================
    // EVENTS
    // ======================================================================
}