using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class UI_VictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endScreenText;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        Turns.OnVictory += Turns_OnVictory;
    }
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Shows the victory screen with the winning team.
    /// </summary>
    /// <param name="winnerTeam"></param>
    private void DisplayEndScreen(Team winnerTeam)
    {
        endScreen.SetActive(true);
        endScreenText.text = $"{winnerTeam.name} team wins!";
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        DisplayEndScreen(_characters.current.Team());
    }
    
}
