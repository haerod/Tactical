using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

/// <summary>
/// Shows and updates the health bar of the unit on its interface.
/// </summary>
public class UFM_SlicedHealthBar : MonoBehaviour
{
    [SerializeField] private float topDownOffset = 1;
    [SerializeField] private float spacing = 2;
    [SerializeField] private RectTransform lifeLayoutGroup;
    [SerializeField] private GameObject lifeImage;
    [SerializeField] private U_Health health;
    [SerializeField] private U__Unit unit;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Start()
    {
        InitialiseBar();

        _units.OnUnitTurnStart += Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd += Units_OnUnitTurnEnd;
        
        InputEvents.OnUnitEnter += InputEvents_OnUnitEnter;
        InputEvents.OnTileExit += InputEvents_OnTileExit;
        InputEvents.OnTileEnter += InputEvents_OnTileEnter;
        
        health.OnDeath += Health_OnDeath;
        health.HealthChanged += Health_HealthChanged;
    }

    private void OnDisable()
    {
        _units.OnUnitTurnStart -= Units_OnUnitTurnStart;
        _units.OnUnitTurnEnd -= Units_OnUnitTurnEnd;
        
        InputEvents.OnUnitEnter -= InputEvents_OnUnitEnter;
        InputEvents.OnTileExit -= InputEvents_OnTileExit;
        InputEvents.OnTileEnter -= InputEvents_OnTileEnter;
        
        health.OnDeath -= Health_OnDeath;
        health.HealthChanged -= Health_HealthChanged;
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
    
    private void Units_OnUnitTurnStart(object sender, U__Unit startingUnit)
    {
        if(startingUnit != unit)
            return; // Another character

        Show();
    }
    
    private void Units_OnUnitTurnEnd(object sender, U__Unit endingUnit)
    {
        if(endingUnit != unit)
            return; // Another character
        
        Hide();
    }
    
    private void InputEvents_OnUnitEnter(object sender, U__Unit hoveredUnit)
    {
        if(hoveredUnit != unit)
            return; // Another character
        
        U__Unit currentUnit = _units.current;
        
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
        if(_units.current == unit)
            return; // Is the current character
        
        Hide();
    }
    
    private void InputEvents_OnTileEnter(object sender, Tile enteredTile)
    {
        U__Unit currentUnit = _units.current;
        
        if(!currentUnit)
            return; // No current unit
        if(currentUnit.team.IsTeammateOf(unit))
            return; // Teammate
        if(!currentUnit.look.CanSee(unit))
            return; // Not visible
        if(!currentUnit.behavior.playable)
            return; // NPC
        if(!currentUnit.move.movementArea.Contains(enteredTile))
            return; // Tile not in movement area
        if(unit.look.visibleTiles.Contains(enteredTile))
            Hide();
        else
            Show();
    }
}