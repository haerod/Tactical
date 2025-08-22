using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static M__Managers;

public class UI_PercentShootText : MonoBehaviour
{
    [SerializeField] private float percentShootOffset = 50f;
    [Space]
    [SerializeField] private Color zeroColor = Color.grey;
    [SerializeField] private Color basicColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;

    [Header("REFERENCES")]

    [SerializeField] private Text percentShootText;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        _feedback.OnFreeTileEvent += Feedback_OnFreeTile;
        _input.OnNoTile += Input_OnNoTile;
        _feedback.OnHoverEnemy += Feedback_OnHoverEnemy;
        _feedback.OnHoverAlly += Feedback_OnHoverAlly;
        _feedback.OnHoverItself += Feedback_OnHoverItself;
        A_Attack.OnAnyAttackStart += Attack_OnAnyAttackStart;
    }

    private void Update()
    {
        SetPercentShootTextPosition();
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    /// <summary>
    /// Enables the percent shoot text and displays the value in percent.
    /// </summary>
    /// <param name="percent"></param>
    public void SetPercentShootText(int percent)
    {
        percentShootText.gameObject.SetActive(true);
        percentShootText.text = percent + "%";

        switch (percent)
        {
            // 0
            case <= 0:
                percentShootText.color = zeroColor;
                percentShootText.fontStyle = FontStyle.Normal;
                break;
            // Regular
            case < 100:
                percentShootText.color = basicColor;
                percentShootText.fontStyle = FontStyle.Normal;
                break;
            // Critical
            default:
                percentShootText.color = criticalColor;
                percentShootText.fontStyle = FontStyle.Bold;
                break;
        }
    }

    /// <summary>
    /// Disables the percent shoot text.
    /// </summary>
    public void DisablePercentShootText() => percentShootText.gameObject.SetActive(false);

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

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
    
    private void Feedback_OnFreeTile(object sender, Tile tile)
    {
        DisablePercentShootText();
    }
    
    private void Input_OnNoTile(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void Feedback_OnHoverEnemy(object sender, C__Character enemy)
    {
        C__Character currentCharacter = _characters.current;
        
        if(!currentCharacter.attack.AttackableTiles().Contains(enemy.tile))
            return;
        
        SetPercentShootText(currentCharacter.attack.GetPercentToTouch(
            currentCharacter.look.GetTilesOfLineOfSightOn(enemy.tile.coordinates).Count,
            enemy.cover.GetCoverProtectionValueFrom(enemy.look)));   
    }
    
    private void Feedback_OnHoverItself(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }

    private void Feedback_OnHoverAlly(object sender, C__Character ally)
    {
        DisablePercentShootText();
    }
    
    private void Attack_OnAnyAttackStart(object sender, EventArgs e)
    {
        DisablePercentShootText();
    }
}