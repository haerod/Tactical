using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using static M__Managers;

public class Module_PercentShootText : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] private float textOffset = 20f;
    [Space]
    [SerializeField] private GameColor zeroColor;
    [SerializeField] private GameColor basicColor;
    [SerializeField] private GameColor criticalColor;

    [Header("REFERENCES")]

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI percentShootText;

    private Unit currentUnit;
    
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
        if(currentUnit)
            currentUnit.attack.OnAttackStart -= Attack_OnAttackStart;
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
        panel.SetActive(true);
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
                percentShootText.fontStyle = FontStyles.Bold;
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
    private void DisablePercentShootText() => panel.SetActive(false);
    
    /// <summary>
    /// Sets the text position at mouse position + offset.
    /// </summary>
    private void SetPercentShootTextPosition()
    {
        panel.transform.position = Input.mousePosition + Vector3.right * (Screen.width * textOffset/100);
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Units_OnUnitTurnStart(object sender, Unit startingUnit)
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

    private void Units_OnUnitTurnEnd(object sender, Unit endingUnit)
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

    private void InputEvents_OnEnemyEnter(object sender, Unit enemy)
    {
        Unit current = _units.current;
        
        if(!current.CanPlay())
            return; // Unit can't play
        if(!current.actionsHolder.HasAvailableAction<A_Attack>())
            return; // No usable attack action
        if(!current.attack.AttackableTiles().Contains(enemy.tile))
            return; // Enemy not visible
        
        SetPercentShootText(current.attack.GetChanceToTouch(enemy));   
    }
    
    private void InputEvents_OnCurrentUnitEnter(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void InputEvents_OnAllyEnter(object sender, Unit ally)
    {
        DisablePercentShootText();
    }
    
    private void Attack_OnAttackStart(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }
    
    private void InputEvents_OnUnitExit(object sender, Unit exitedUnit)
    {
        DisablePercentShootText();
    }
}