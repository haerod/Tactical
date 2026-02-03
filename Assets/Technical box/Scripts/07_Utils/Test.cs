using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static M__Managers;

public class Test : MonoBehaviour
{
    public static Test instance;
    
    public Inventory inventory;
    public Item toAdd;
    
    private void Awake()
    {
        instance = this;
        // GameEvents.OnAnyResistancesTriggered += GameEventsOnOnAnyResistancesTriggered;
        // GameEvents.OnAnyWeaknessesTriggered += GameEventsOnOnAnyWeaknessesTriggered;
    }
    
    private void GameEventsOnOnAnyWeaknessesTriggered(object sender, GameEvents.DamageTypeTriggerEventArgs e)
    {        
        print("WEAKNESSES");
        e.damageTypes.Print();
    }
    
    private void GameEventsOnOnAnyResistancesTriggered(object sender, GameEvents.DamageTypeTriggerEventArgs e)
    {
        print("RESISTANCES");
        e.damageTypes.Print();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            
        }
    }
}
