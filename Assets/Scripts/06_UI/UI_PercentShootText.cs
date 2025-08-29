using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class UI_PercentShootText : MonoBehaviour
{
    [SerializeField] private float percentShootOffset = 50f;
    [Space]
    [SerializeField] private Color zeroColor = Color.grey;
    [SerializeField] private Color basicColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;

    [Header("REFERENCES")]

    [SerializeField] private TextMeshProUGUI percentShootText;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        InputEvents.OnNoTile += InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter += InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnAllyEnter += InputEvents_OnAllyEnter;
        InputEvents.OnItselfEnter += InputEvents_OnItselfEnter;
        A_Attack.OnAnyAttackStart += Attack_OnAnyAttackStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        Turns.OnVictory += Turns_OnVictory;
    }
    
    private void Update()
    {
        SetPercentShootTextPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Enables the percent shoot text and displays the value in percent.
    /// </summary>
    /// <param name="percent"></param>
    private void SetPercentShootText(int percent)
    {
        percentShootText.gameObject.SetActive(true);
        percentShootText.text = percent + "%";

        switch (percent)
        {
            // 0
            case <= 0:
                percentShootText.color = zeroColor;
                percentShootText.fontStyle = FontStyles.Normal;
                break;
            // Regular
            case < 100:
                percentShootText.color = basicColor;
                percentShootText.fontStyle = FontStyles.Normal;
                break;
            // Critical
            default:
                percentShootText.color = criticalColor;
                percentShootText.fontStyle = FontStyles.Bold;
                break;
        }
    }

    /// <summary>
    /// Disables the percent shoot text.
    /// </summary>
    private void DisablePercentShootText() => percentShootText.gameObject.SetActive(false);
    
    /// <summary>
    /// Sets the text position at mouse position + offset.
    /// </summary>
    private void SetPercentShootTextPosition()
    {
        percentShootText.transform.position = Input.mousePosition;
        percentShootText.transform.position += Vector3.right * percentShootOffset;
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void InputEvents_OnFreeTileEnter(object sender, Tile tile)
    {
        DisablePercentShootText();
    }
    
    private void InputEvents_OnNoTile(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnEnemyEnter(object sender, C__Character enemy)
    {
        C__Character currentCharacter = _characters.current;
        
        if(!currentCharacter.attack.AttackableTiles().Contains(enemy.tile))
            return;
        
        SetPercentShootText(currentCharacter.attack.GetPercentToTouch(
            currentCharacter.look.GetTilesOfLineOfSightOn(enemy.tile.coordinates).Count,
            enemy.cover.GetCoverProtectionValueFrom(enemy.look)));   
    }
    
    private void InputEvents_OnItselfEnter(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnAllyEnter(object sender, C__Character ally)
    {
        DisablePercentShootText();
    }
    
    private void Attack_OnAnyAttackStart(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingTurnCharacter)
    {
        DisablePercentShootText();
    }
    
    private void Turns_OnVictory(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

}