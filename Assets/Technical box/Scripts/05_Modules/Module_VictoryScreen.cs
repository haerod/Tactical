using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using static M__Managers;

public class Module_VictoryScreen : MonoBehaviour
{
    [SerializeField] private int sceneIndexToLoad = -1;
    
    [Header("- REFERENCES -")][Space]
    
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _endScreenText;
    [SerializeField] private GameObject _nextLevelButton;

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
    /// Called by Restart button.
    /// </summary>
    public void ClickOnReplay() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
    /// <summary>
    /// Loads the next level.
    /// Called by Next Level button.
    /// </summary>
    public void ClickOnNextLevel() => SceneManager.LoadScene(sceneIndexToLoad);
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the victory screen with the winning team.
    /// </summary>
    /// <param name="text"></param>
    private void DisplayEndScreen(string text, bool isVictory)
    {
        _panel.SetActive(true);
        _endScreenText.text = text;

        if (isVictory && sceneIndexToLoad > 0)
            _nextLevelButton.gameObject.SetActive(true);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Level_OnVictory(object sender, EventArgs e)
    {
        DisplayEndScreen($"Victory", true);
    }
    
    private void Level_OnDefeat(object sender, EventArgs e)
    {
        DisplayEndScreen($"Defeat", false);
    }
}