using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class UI_SlicedHealthBar : MonoBehaviour
{
    [SerializeField] private float topDownOffset = 1;
    [SerializeField] private float spacing = 2;
    [SerializeField] private RectTransform lifeLayoutGroup;
    [SerializeField] private GameObject lifeImage;
    [SerializeField] private C_Health health;
    [SerializeField] private C__Character unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        InitialiseBar();

        _characters.OnCharacterTurnStart += Characters_OnCharacterTurnStart;
        _characters.OnCharacterTurnEnd += Characters_OnCharacterTurnEnd;
        
        InputEvents.OnCharacterEnter += InputEvents_OnCharacterEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        
        health.OnDeath += Health_OnDeath;
        health.HealthChanged += Health_HealthChanged;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    /// <summary>
    /// Displays the current life and max life on life bar.
    /// </summary>
    private void InitialiseBar()
    {
        DisplayMaxHealth();
        DisplayCurrentHealth();
    }
    
    /// <summary>
    /// Enables the life bar.
    /// </summary>
    private void Show () => lifeLayoutGroup.gameObject.SetActive(true);
    
    /// <summary>
    /// Disables the life bar.
    /// </summary>
    private void Hide() => lifeLayoutGroup.gameObject.SetActive(false);

    /// <summary>
    /// Displays max life on health bar.
    /// </summary>
    private void DisplayMaxHealth()
    {
        // Destroy ancient objects
        for (int i = 0; i < lifeLayoutGroup.childCount; i++)
        {
            Destroy(lifeLayoutGroup.GetChild(i).gameObject);
        }

        // Display life
        for (int i = 0; i < health.health; i++)
        {
            RectTransform rt = Instantiate(lifeImage, lifeLayoutGroup).GetComponent<RectTransform>();
            float parentWidth = lifeLayoutGroup.rect.width;
            float lifeWidth = 0;

            // Size
            Vector2 sizeDeltaAdjusted = new Vector2(
                (parentWidth) / health.health - (spacing * 2),
                lifeLayoutGroup.rect.height - topDownOffset *2);
            rt.sizeDelta = sizeDeltaAdjusted;

            lifeWidth = sizeDeltaAdjusted.x;

            // Position
            Vector2 localPosition = new Vector2(
                ((parentWidth / health.health) * i) - (parentWidth / 2) + (lifeWidth / 2) + spacing / (health.health) + spacing,
                0);
            rt.localPosition = localPosition;
        }
    }

    /// <summary>
    /// Displays the current life on life bar.
    /// </summary>
    private void DisplayCurrentHealth()
    {
        for (int i = 0; i < health.health; i++)
        {
            GameObject currentLifeBar = lifeLayoutGroup.GetChild(i).gameObject;
            currentLifeBar.SetActive(i < health.currentHealth);
        }
    }
    
    /// <summary>
    /// Starts a waits for "time" seconds and executes an action.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    private void Wait(float time, Action onEnd) => StartCoroutine(Wait_Co(time, onEnd));

    /// <summary>
    /// Waits for "time" seconds and executes an action.
    /// Called by Wait() method.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="onEnd"></param>
    /// <returns></returns>
    private IEnumerator Wait_Co(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);

        onEnd();
    }
    
    // ======================================================================
    // EVENTS
    // ======================================================================
    
    private void Characters_OnCharacterTurnStart(object sender, C__Character startingUnit)
    {
        if(startingUnit != unit)
            return; // Another character

        Show();
    }
    
    private void Characters_OnCharacterTurnEnd(object sender, C__Character endingUnit)
    {
        if(endingUnit != unit)
            return; // Another character
        
        Hide();
    }
    
    private void InputEvents_OnCharacterEnter(object sender, C__Character hoveredUnit)
    {
        if(hoveredUnit != unit)
            return; // Another character
        
        C__Character currentUnit = _characters.current;
        
        if(currentUnit == unit)
            return; // Current unit
        if(!currentUnit.look.CharactersVisibleInFog().Contains(unit))
            return; // Invisible unit
        
        Show();
    }
    
    private void Health_OnDeath(object sender, EventArgs e)
    {
        Wait(1, Hide);
    }
    
    private void Health_HealthChanged(object sender, EventArgs e)
    {
        DisplayCurrentHealth();
    }
    
    private void InputEvents_OnTileExit(object sender, Tile exitedTile)
    {
        if(exitedTile.character != unit)
            return; // Not this character
        if(_characters.current == unit)
            return; // Is the current character
        
        Hide();
    }

}
