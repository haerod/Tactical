﻿using UnityEngine;
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
    [Space]
    [SerializeField] private Button nextTurnButton = null;
    [Space]
    [SerializeField] private GameObject endScreen = null;
    [Space]
    public UI_PercentShootText percentText;

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
        endScreenText.text = string.Format("{0} are winners !", winner.team.name);
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

    public bool IsMouseOverUI() => EventSystem.current.IsPointerOverGameObject();

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
}
