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
    [SerializeField] private Text debugCoordinatesText = null;
    [Space]
    [SerializeField] private Button addActionPointButton = null;
    [SerializeField] private Button nextTurnButton = null;
    [SerializeField] private Button saveBoardButton = null;
    [Space]
    [SerializeField] private Toggle autoSaveToggle = null;
    [Space]    
    [SerializeField] private Dropdown creatorTutorialsDropdown = null;
    [Space]
    [SerializeField] private GameObject actionPointsObject = null;
    [SerializeField] private GameObject endScreen = null;
    [SerializeField] private GameObject canvasBoardCreation = null;
    [SerializeField] private GameObject saveFeedbackText = null;
    [Space]
    [SerializeField] private Transform tutoParent = null;
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

    private void Start()
    {
        if (_creator.editMode)
            CheckAutoSaveOnStart();
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
        if (c != _characters.currentCharacter) return;

        actionPointsText.text = text;
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
        _characters.currentCharacter.actionPoints.AddActionPoints();
        _characters.currentCharacter.ClearTilesFeedbacks();
        _characters.currentCharacter.EnableTilesFeedbacks();
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

    // MISC
    // ====

    /// <summary>
    /// Enable / disable player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    public void SetTurnPlayerUIActive(bool value)
    {
        SetActiveCheatButton(value);
        nextTurnButton.gameObject.SetActive(value);
        SetActiveActionPointsUI(value);
    }
    
    /// <summary>
    /// Enable / disable player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActionPlayerUIActive(bool value)
    {
        if (value == true && !_characters.currentCharacter.behavior.playable) return; // EXIT : it's not player's turn

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

    // BOARD CREATOR
    // =============

    /// <summary>
    /// Enable/Disable UI for creator mode.
    /// </summary>
    /// <param name="value"></param>
    public void SetCreatorModeUIActive(bool value)
    {
        canvasBoardCreation.SetActive(value);
    }

    /// <summary>
    /// Enable/Disable coordinates text and show the values.
    /// </summary>
    public void SetDebugCoordinatesTextActive(bool value, int x = 0, int y = 0)
    {
        debugCoordinatesText.gameObject.SetActive(value);

        if (!value) return;

        debugCoordinatesText.text = x + " , " + y;
    }

    /// <summary>
    /// Save the current board in data.
    /// Relied to the event on the button Save board.
    /// </summary>
    public void ClickOnSave()
    {
        _board.SaveBoard();

        if (!_board.data) return; // EXIT : No data (case managed in SaveBoard()).

        saveFeedbackText.SetActive(false);
        saveFeedbackText.SetActive(true);
    }

    /// <summary>
    /// Enable/Disable autosave.
    /// Relied to the event on the toggle Auto save.
    /// If is no data, autosave is automatically off.
    /// </summary>
    public void ToggleAutoSave()
    {
        if(_board.data)
        {
            _creator.autoSave = autoSaveToggle.isOn;

            if (_creator.autoSave)
            {
                _board.SaveBoard();
                saveBoardButton.gameObject.SetActive(false);
            }
            else
            {
                saveBoardButton.gameObject.SetActive(true);
            }
        }
        else
        {
            _creator.autoSave = false;
            autoSaveToggle.isOn = false;
            saveBoardButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// DisableAutoSave if board data is empty.
    /// </summary>
    public void CheckAutoSaveOnStart()
    {
        if (!_board.data)
        {
            _creator.autoSave = false;
            autoSaveToggle.isOn = false;
            saveBoardButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Set the start value of autosave toggle.
    /// Called by M_Creator.
    /// </summary>
    public void SetStartToggleValue()
    {
        autoSaveToggle.isOn = _creator.autoSave;

        if (_creator.autoSave)
        {
            saveBoardButton.gameObject.SetActive(false);
        }
        else
        {
            saveBoardButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Enable the dropdown tutorials.
    /// </summary>
    public void EnableDropdownTutorial()
    {
        foreach (Transform tuto in tutoParent)
        {
            if(tuto)
                tuto.gameObject.SetActive(false);
        }

        if (creatorTutorialsDropdown.value == 0) return; // EXIT : Starting option (choose a tutorial).

        tutoParent.GetChild(creatorTutorialsDropdown.value - 1).gameObject.SetActive(true);
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
