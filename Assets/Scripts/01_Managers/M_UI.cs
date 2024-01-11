using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;
using System;

public class M_UI : MonoBehaviour
{
    [Header("REFERENCES")]

    [SerializeField] private Text actionPointsText = null;
    [SerializeField] private Text endScreenText = null;
    [Space]
    [SerializeField] private Button addActionPointButton = null;
    [SerializeField] private Button nextTurnButton = null;
    [Space]
    [SerializeField] private GameObject actionPointsObject = null;
    [SerializeField] private GameObject endScreen = null;
    [Space]
    public UI_PercentShootText percentText;
    public UI_ActionCostText actionCostText;

    public static M_UI instance;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There is more than one M_UI in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
        }
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Write text in action point text if the given character is the current character.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="c"></param>
    public void SetActionPointText(string text, C__Character c)
    {
        if (c != _characters.current) return;

        actionPointsText.text = text;
    }

    /// <summary>
    /// Enable / disable player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Turn(bool value)
    {
        SetActiveCheatButton(value);
        nextTurnButton.gameObject.SetActive(value);
        SetActiveActionPointsUI(value);
    }
    
    /// <summary>
    /// Enable / disable player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Action(bool value)
    {
        if (value == true && !_characters.current.behavior.playable) return; // EXIT : it's not player's turn

        SetActiveCheatButton(value);
        nextTurnButton.gameObject.SetActive(value);
    }

    /// <summary>
    /// Enable the end screen (explaining which team is the winner).
    /// </summary>
    /// <param name="winner"></param>
    public void EnableEndScreen(C__Character winner)
    {
        endScreen.SetActive(true);
        endScreenText.text = string.Format("{0} are winners !", _rules.teamInfos[winner.infos.team].teamName);
    }

    // BUTTONS
    // =======

    /// <summary>
    /// Add an action point to the current character.
    /// Relied to the event on the button Add action point.
    /// (CHEAT)
    /// </summary>
    public void ClickOnAddActionPoint()
    {
        _characters.current.actionPoints.AddActionPoints();
        _characters.current.ClearTilesFeedbacks();
        _characters.current.EnableTilesFeedbacks();
    }

    /// <summary>
    /// Restart the scene.
    /// Relied to the event on the button Replay.
    /// </summary>
    public void ClickOnReplay()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Active or desactive Action Points Object (depending the Rules).
    /// </summary>
    /// <param name="value"></param>
    private void SetActiveActionPointsUI(bool value)
    {
        actionPointsObject.SetActive(value);

        if (_rules.actionsByTurn == M_Rules.ActionsByTurn.OneActionByTurn)
            actionPointsObject.SetActive(false);
    }

    /// <summary>
    /// Active or desactive Action Points Object (depending the Rules).
    /// </summary>
    /// <param name="value"></param>
    private void SetActiveCheatButton(bool value)
    {
        addActionPointButton.gameObject.SetActive(value);

        if (_rules.actionsByTurn == M_Rules.ActionsByTurn.OneActionByTurn)
            addActionPointButton.gameObject.SetActive(false);
    }
}
