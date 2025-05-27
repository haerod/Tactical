using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static M__Managers;

public class M_PlayerActions : MonoBehaviour
{
    public static M_PlayerActions instance;

    private Tile pointedTile;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    private void Awake()
    {
        // Singleton
        if (!instance)
            instance = this;
        else
            Debug.LogError(
                "There is more than one M_PlayerActions in the scene, kill this one.\n(error by Basic Unity Tactical Tool)", gameObject);
    }

    private void Start()
    {
        _input.OnEnterTile += Input_OnEnterTile;
        _input.OnExitTile += Input_OnExitTile;
        _input.OnEnterCharacter += Input_OnEnterCharacter;
        _input.OnExitCharacter += Input_OnExitCharacter;
    }

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private void Input_OnEnterCharacter(object sender, C__Character character)
    {
        
    }

    private void Input_OnExitCharacter(object sender, C__Character character)
    {
        
    }

    private void Input_OnEnterTile(object sender, Tile tile)
    {
        
    }
    
    private void Input_OnExitTile(object sender, Tile tile)
    {
       
    }


}
