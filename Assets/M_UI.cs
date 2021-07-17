using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoSingleton<M_UI>
{
    [Header("REFERENCES")]

    [SerializeField] private Text actionPointsText = null;
    [SerializeField] private Text actionCostText = null;
    [Space]
    [SerializeField] private Button followButton = null;
    [SerializeField] private Button addActionPointButton = null;
    [Space]
    [SerializeField] private GameObject actionPointsObject = null;

    [Header("ACTION COST TEXT SETTINGS")]

    [SerializeField] private float actionCostOffset = 30f;
    [SerializeField] private Color inRangeColor = Color.yellow;
    [SerializeField] private Color outRangeColor = Color.grey;

    private Camera cam;
    private Transform actionCostTarget;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!actionCostTarget) return;

        actionCostText.transform.position = cam.WorldToScreenPoint(actionCostTarget.position);
        actionCostText.transform.position += Vector3.up * actionCostOffset;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public void SetActionPointText(string text, Character c)
    {
        if (c != _characters.IsCurrentCharacter(c)) return;

        actionPointsText.text = text;
    }

    public void SetActionCostText(string value, Transform target, bool outRange = false)
    {
        actionCostText.gameObject.SetActive(true);
        actionCostText.text = value;
        actionCostTarget = target;

        if (outRange)
        {
            actionCostText.color = outRangeColor;
        }
        else
        {
            actionCostText.color = inRangeColor;
        }
    }

    public void DisableActionCostText()
    {
        actionCostText.gameObject.SetActive(false);
    }

    public void SetPlayerUIActive(bool value)
    {
        addActionPointButton.gameObject.SetActive(value);
        followButton.gameObject.SetActive(value);
        actionPointsObject.SetActive(value);
    }

    // CHEATS
    // ======

    public void ClickOnAddActionPoint()
    {
        _characters.currentCharacter.actionPoints.AddActionPoints();
        _characters.currentCharacter.ClearTilesFeedbacks();
        _characters.currentCharacter.EnableTilesFeedbacks();
    }

    public void ClickOnFollow()
    {
        Character c = _characters.currentCharacter;

        c.behaviour.PlayBehaviour();
    }

    public void CheckFollowButton()
    {
        followButton.interactable = _characters.currentCharacter.behaviour.target != null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
