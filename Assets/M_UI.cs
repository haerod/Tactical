using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoSingleton<M_UI>
{
    [Header("ACTION COST TEXT SETTINGS")]

    [SerializeField] private float actionCostOffset = 30f;
    [SerializeField] private Color inRangeColor = Color.yellow;
    [SerializeField] private Color outRangeColor = Color.grey;

    [Header("REFERENCES")]

    [SerializeField] private Text actionPointsText = null;
    [SerializeField] private Text actionCostText = null;
    [SerializeField] private Text endScreenText = null;
    [Space]
    [SerializeField] private Button addActionPointButton = null;
    [SerializeField] private Button nextTurnButton = null;
    [Space]
    [SerializeField] private GameObject actionPointsObject = null;
    [Space]
    [SerializeField] private GameObject endScreen = null;

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

    // Enable / disable player's UI out their turn 
    public void SetTurnPlayerUIActive(bool value)
    {
        addActionPointButton.gameObject.SetActive(value);
        actionPointsObject.SetActive(value);
        nextTurnButton.gameObject.SetActive(value);
    }

    // Enable / disable player's UI during their actions
    public void SetActionPlayerUIActive(bool value)
    {
        if (value == true && !_characters.currentCharacter.behaviour.playable) return; // EXIT : it's not player's turn

        addActionPointButton.gameObject.SetActive(value);
        nextTurnButton.gameObject.SetActive(value);
    }

    public void EnableEndScreen(Character winner)
    {
        endScreen.SetActive(true);
        endScreenText.text = string.Format("{0} are winners !", _rules.teamInfos[winner.infos.team].teamName);
    }

    public void ClickOnReplay()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    // CHEATS
    // ======

    public void ClickOnAddActionPoint()
    {
        _characters.currentCharacter.actionPoints.AddActionPoints();
        _characters.currentCharacter.ClearTilesFeedbacks();
        _characters.currentCharacter.EnableTilesFeedbacks();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
