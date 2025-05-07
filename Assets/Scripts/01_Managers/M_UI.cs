using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using static M__Managers;

public class M_UI : MonoBehaviour
{
    [Header("REFERENCES")]

    [SerializeField] private Text endScreenText = null;
    [SerializeField] private Button nextTurnButton = null;
    [SerializeField] private GameObject endScreen = null;
    [SerializeField] private UI_PercentShootText percentText;

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
        _characters.GetCharacterList()
            .ForEach(character => DisplayCharacterCoverState(character, character.cover.GetCoverState()));
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enables / disables player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Turn(bool value) => nextTurnButton.gameObject.SetActive(value);

    /// <summary>
    /// Enables / disables player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Action(bool value)
    {
        if (value == true && !_characters.current.behavior.playable) return; // EXIT : it's not player's turn

        nextTurnButton.gameObject.SetActive(value);
    }

    /// <summary>
    /// Enables the end screen (explaining which team wins).
    /// </summary>
    /// <param name="winner"></param>
    public void EnableEndScreen(C__Character winner)
    {
        endScreen.SetActive(true);
        endScreenText.text = $"{winner.team.name} are winners !";
    }

    /// <summary>
    /// Enables percent shoot text and sets the value.
    /// </summary>
    /// <param name="percent"></param>
    public void ShowPercentText(int percent) => percentText.SetPercentShootText(percent);

    /// <summary>
    /// Disables percent shoot text.
    /// </summary>
    public void HidePercentText()
    {
        percentText.DisablePercentShootText();
    }

    /// <summary>
    /// Restarts the scene.
    /// Relied to the event on the button Replay.
    /// </summary>
    public void ClickOnReplay() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    /// <summary>
    /// Returns true if pointer is over UI. Else, returns false.
    /// </summary>
    /// <returns></returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public void DisplayCharacterCoverState(C__Character character, CoverInfo coverInfo)
    {
        if(coverInfo == null)
            character.coverState.HideCoverState();
        else
            character.coverState.DisplayCoverState(
                coverInfo.GetCoverType(), 
                coverInfo.GetIsCovered() ? _feedback.GetCoveredColour() : _feedback.GetUncoveredColour());
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
