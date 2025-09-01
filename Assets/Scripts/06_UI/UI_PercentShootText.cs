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
        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
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
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingCharacter)
    {
        if(!startingCharacter.behavior.playable)
            return; // NPC
        
        startingCharacter.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnNoTile += InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter += InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnAllyEnter += InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter += InputEvents_OnCurrentUnitEnter;
    }

    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingCharacter)
    {
        if(!endingCharacter.behavior.playable)
            return; // NPC
        
        endingCharacter.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnNoTile -= InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter -= InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnAllyEnter -= InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter -= InputEvents_OnCurrentUnitEnter;
        
        DisablePercentShootText();
    }
    
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
        
        if(!currentCharacter.CanPlay())
            return; // Unit can't play
        
        if(!currentCharacter.attack.AttackableTiles().Contains(enemy.tile))
            return; // Enemy not visible
        
        SetPercentShootText(currentCharacter.attack.GetChanceToTouch(
            currentCharacter.look.GetTilesOfLineOfSightOn(enemy.tile.coordinates).Count,
            enemy.cover.GetCoverProtectionValueFrom(enemy.look)));   
    }
    
    private void InputEvents_OnCurrentUnitEnter(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnAllyEnter(object sender, C__Character ally)
    {
        DisablePercentShootText();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }
}