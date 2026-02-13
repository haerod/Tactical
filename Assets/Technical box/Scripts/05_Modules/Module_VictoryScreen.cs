using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using static M__Managers;

public class Module_VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endScreenText;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        _level.OnVictory += Level_OnVictory;
        _level.OnDefeat += Level_OnDefeat;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Restarts the scene.
    /// Relied to the event on the button Replay.
    /// </summary>
    public void ClickOnReplay() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the victory screen with the winning team.
    /// </summary>
    /// <param name="text"></param>
    private void DisplayEndScreen(string text)
    {
        endScreen.SetActive(true);
        endScreenText.text = text;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Level_OnVictory(object sender, EventArgs e)
    {
        DisplayEndScreen($"Victory");
    }
    
    private void Level_OnDefeat(object sender, EventArgs e)
    {
        DisplayEndScreen($"Defeat");
    }
}
