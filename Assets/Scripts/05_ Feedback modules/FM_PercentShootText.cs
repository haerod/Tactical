using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static M__Managers;

public class FM_PercentShootText : MonoBehaviour
{
    [SerializeField] private float percentShootOffset = 50f;
    [Space]
    [SerializeField] private GameColor zeroColor;
    [SerializeField] private GameColor basicColor;
    [SerializeField] private GameColor criticalColor;

    [Header("REFERENCES")]

    [SerializeField] private TextMeshProUGUI percentShootText;

    private U__Unit currentUnit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
    }

    private void OnDisable()
    {
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnNoTile -= InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter -= InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnUnitExit -= InputEvents_OnUnitExit;
        InputEvents.OnAllyEnter -= InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter -= InputEvents_OnCurrentUnitEnter;
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
                percentShootText.color = zeroColor.color;
                percentShootText.fontStyle = FontStyles.Normal;
                break;
            // Regular
            case < 100:
                percentShootText.color = basicColor.color;
                percentShootText.fontStyle = FontStyles.Normal;
                break;
            // Critical
            default:
                percentShootText.color = criticalColor.color;
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
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(!startingUnit.behavior.playable)
            return; // NPC

        currentUnit = startingUnit;
        
        currentUnit.attack.OnAttackStart += Attack_OnAttackStart;
        InputEvents.OnNoTile += InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter += InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter += InputEvents_OnEnemyEnter;
        InputEvents.OnUnitExit += InputEvents_OnUnitExit;
        InputEvents.OnAllyEnter += InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter += InputEvents_OnCurrentUnitEnter;
    }

    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(!endingUnit.behavior.playable)
            return; // NPC
        
        currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
        InputEvents.OnNoTile -= InputEvents_OnNoTile;
        InputEvents.OnFreeTileEnter -= InputEvents_OnFreeTileEnter;
        InputEvents.OnEnemyEnter -= InputEvents_OnEnemyEnter;
        InputEvents.OnUnitExit -= InputEvents_OnUnitExit;
        InputEvents.OnAllyEnter -= InputEvents_OnAllyEnter;
        InputEvents.OnCurrentUnitEnter -= InputEvents_OnCurrentUnitEnter;
        
        DisablePercentShootText();
        
        currentUnit = null;
    }
    
    private void InputEvents_OnFreeTileEnter(object sender, Tile tile)
    {
        DisablePercentShootText();
    }
    
    private void InputEvents_OnNoTile(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnEnemyEnter(object sender, U__Unit enemy)
    {
        U__Unit current = _units.current;
        
        if(!current.CanPlay())
            return; // Unit can't play
        
        if(!current.attack.AttackableTiles().Contains(enemy.tile))
            return; // Enemy not visible
        
        SetPercentShootText(current.attack.GetChanceToTouch(enemy));   
    }
    
    private void InputEvents_OnCurrentUnitEnter(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnAllyEnter(object sender, U__Unit ally)
    {
        DisablePercentShootText();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }
    
    private void InputEvents_OnUnitExit(object sender, U__Unit exitedUnit)
    {
        DisablePercentShootText();
    }
}