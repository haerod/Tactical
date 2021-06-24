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

    // CHEATS
    // ======

    public void ClickOnAddActionPoint()
    {
        _characters.currentCharacter.actionPoints.AddActionPoints();
        _characters.currentCharacter.move.ClearAreaZone();
        _characters.currentCharacter.move.EnableMoveArea();
    }

    public void ClickOnFollow()
    {
        Character c = _characters.currentCharacter;

        if (c.behaviour.target == null)
        {
            Debug.LogError("mauvais perso : " + c.name);
            return;
        }

        if(c.behaviour.target == c)
        {
            Debug.LogError("oops, target is character itself");
            return;
        }

        List<Tile> path = new List<Tile>();
        path = _pathfinding.PathfindAround(
                c.GetTile(),
                c.behaviour.target.GetTile(),
                _rules.canPassAcross == M_GameRules.PassAcross.Nobody);

        if (Utils.IsVoidList(path))
        {
            Debug.LogError("no path");
            return;
        }

        c.move.MoveOnPath(path);
    }

    public void CheckFollowButton()
    {
        followButton.interactable = _characters.currentCharacter.behaviour.target != null;
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
