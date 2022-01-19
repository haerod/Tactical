using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoBehaviour
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
        cam = Camera.main;
    }

    private void Update()
    {
        SetActionPointTextPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ACTION POINT TEXT
    // =================

    /// <summary>
    /// Write text in action point text if the given character is the current character.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="c"></param>
    public void SetActionPointText(string text, Character c)
    {
        if (c != _characters.IsCurrentCharacter(c)) return;

        actionPointsText.text = text;
    }

    /// <summary>
    /// Write text in action cost text and specify its target and its color.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="target"></param>
    /// <param name="outRange"></param>
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

    /// <summary>
    /// Disable the action point text.
    /// </summary>
    public void DisableActionCostText()
    {
        actionCostText.gameObject.SetActive(false);
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
        addActionPointButton.gameObject.SetActive(value);
        actionPointsObject.SetActive(value);
        nextTurnButton.gameObject.SetActive(value);
    }
    
    /// <summary>
    /// Enable / disable player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActionPlayerUIActive(bool value)
    {
        if (value == true && !_characters.currentCharacter.behavior.playable) return; // EXIT : it's not player's turn

        addActionPointButton.gameObject.SetActive(value);
        nextTurnButton.gameObject.SetActive(value);
    }

    /// <summary>
    /// Enable the end screen (explaining which team is the winner).
    /// </summary>
    /// <param name="winner"></param>
    public void EnableEndScreen(Character winner)
    {
        endScreen.SetActive(true);
        endScreenText.text = string.Format("{0} are winners !", _rules.teamInfos[winner.infos.team].teamName);
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Set the position of the action point text relative to its target.
    /// </summary>
    private void SetActionPointTextPosition()
    {
        if (!actionCostTarget) return;

        actionCostText.transform.position = cam.WorldToScreenPoint(actionCostTarget.position);
        actionCostText.transform.position += Vector3.up * actionCostOffset;
    }
}
