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

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enable / disable player's UI out of its turn.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Turn(bool value)
    {
        nextTurnButton.gameObject.SetActive(value);
    }
    
    /// <summary>
    /// Enable / disable player's UI during its actions.
    /// </summary>
    /// <param name="value"></param>
    public void SetActivePlayerUI_Action(bool value)
    {
        if (value == true && !_characters.current.behavior.playable) return; // EXIT : it's not player's turn

        nextTurnButton.gameObject.SetActive(value);
    }

    /// <summary>
    /// Enable the end screen (explaining which team is the winner).
    /// </summary>
    /// <param name="winner"></param>
    public void EnableEndScreen(C__Character winner)
    {
        endScreen.SetActive(true);
        endScreenText.text = $"{winner.team.name} are winners !";
    }

    /// <summary>
    /// Enable percent shoot text and set the value.
    /// </summary>
    /// <param name="percent"></param>
    public void ShowPercentText(int percent)
    {
        percentText.SetPercentShootText(percent);
    }

    /// <summary>
    /// Disable percent shoot text.
    /// </summary>
    public void HidePercentText()
    {
        percentText.DisablePercentShootText();
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

    /// <summary>
    /// Return true if pointer is over UI. Else, return false.
    /// </summary>
    /// <returns></returns>
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
