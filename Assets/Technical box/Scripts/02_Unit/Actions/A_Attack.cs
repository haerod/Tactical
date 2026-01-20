using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class A_Attack : A__Action
{
    [Header("PARAMETERS")]
    
    [SerializeField] private int precision = 100;
    
    private Action onAttackDone;

    public event EventHandler OnAttackStart;
    public event EventHandler OnAttackEnd;
    public event EventHandler<Unit> OnAttackMiss;
    public event EventHandler<Unit> OnAttackableEnemyHovered;
    public event EventHandler<Unit> OnUnitExit;

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================
    
    /// <summary>
    /// Returns the attackable tiles, depending on the rules.
    /// </summary>
    /// <returns></returns>
    public List<Tile> AttackableTiles() => unit.look.EnemiesVisibleInFog()
            .Where(testedUnit => IsTileInRange(testedUnit.tile))
            .Select(testedUnit => testedUnit.tile)
            .ToList();
    
    /// <summary>
    /// Attacks the target and starts an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    public void Attack(Unit currentTarget)
    {
        if (!unit.look.CanSee(currentTarget)) 
            return; // Enemy not in sight

        Unit target = currentTarget;

        unit.move.OrientTo(target.transform.position);
        target.move.OrientTo(unit.transform.position);

        int damages = unit.weaponHolder.weaponData.RandomDamages();

        StartAction();
        OnAttackStart?.Invoke(this, EventArgs.Empty);
        GameEvents.InvokeOnAnyAttackStart(unit);
        
        int percentOfTouch = GetChanceToTouch(currentTarget);
        
        if (UnityEngine.Random.Range(0, 101) < percentOfTouch) // SUCCESS
            SetOnAttackDone(true, damages, target);
        else // MISS
            SetOnAttackDone(false, 0, target);
    }
    
    /// <summary>
    /// Ends the attack.
    /// Called by C_AnimatorScripts, after the shoot animation.
    /// </summary>
    public void EndAttack()
    {
        _camera.Shake();
        onAttackDone();
    }
    
    /// <summary>
    /// Returns the percent of chance to touch the target, including the reduction by distance.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public int GetChanceToTouch(Unit target)
    {
        int precisionToReturn = precision;
        WeaponData currentWeaponData = unit.weaponHolder.weaponData;
        int targetProtectionValue = target.cover.GetCoverProtectionValueFrom(unit);
        
        if(!currentWeaponData.isMeleeWeapon)
            precisionToReturn -= targetProtectionValue;
        
        for (int i = 0; i < unit.look.GetTilesOfLineOfSightOn(target.coordinates).Count; i++)
            precisionToReturn -= currentWeaponData.precisionMalusByDistance;

        if(targetProtectionValue < 100)
            precisionToReturn += currentWeaponData.precisionModifier;
        
        return Mathf.Clamp(precisionToReturn, 0, 100);
    }
    
    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================
    
    /// <summary>
    /// Sets OnAttackDone lambda.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="damages"></param>
    /// <param name="target"></param>
    private void SetOnAttackDone(bool success, int damages, Unit target)
    {
        onAttackDone = () =>
        {
            if (success)
            {
                target.health.AddDamages(damages, unit.weaponHolder.weaponData.damageType);
            }
            else
            {
                OnAttackMiss?.Invoke(this, target);
                target.anim.StartDodge();
            }
            
            Wait(0.5f, () =>
            {
                OnAttackEnd?.Invoke(this, EventArgs.Empty);

                EndAction();
            });
        };
    }
    
    /// <summary>
    /// Returns true if the tile is in range.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private bool IsTileInRange(Tile tile)
    {
        WeaponData currentWeaponData = unit.weaponHolder.weaponData;
        List<Tile> los = unit.look.GetTilesOfLineOfSightOn(tile.coordinates);

        if (currentWeaponData.canAttackAnythingInView)
            return unit.look.CanSee(tile.character);
        if (currentWeaponData.isMeleeWeapon)
            return los.Count == 0;
        else
            return los.Count < currentWeaponData.range;
    }
    
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="targetUnit"></param>
    private void EnterLean(Unit targetUnit)
    {
        // if(!unit.cover.AreCoversAround())
        //     return; // No covers around
        //
        // List<Coordinates> aroundCoordinates = _board.GetCoordinatesAround(unit.coordinates, 1, false);
        // List<Tile> tilesToLean = new();
        //
        // foreach (Coordinates testedCoordinate in aroundCoordinates)
        // {
        //     Tile testedTile = _board.GetTileAtCoordinates(testedCoordinate);
        //     
        //     if(!testedTile)
        //         continue; // No tile
        //     if(!unit.move.CanWalkOn(testedTile.type))
        //         continue; // Not walkable
        //     
        //     tilesToLean.Add(testedTile);
        // }
        //
        // foreach (Tile testedTile in tilesToLean)
        // {
        //     if()
        // }
    }
    
    /// <summary>
    /// TODO
    /// </summary>
    private void ExitLean()
    {
        // transform.position = unit.coordinates.ToVector3();
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================
    
    protected override void OnHoverEnemy(Unit hoveredUnit)
    {
        unit.move.OrientTo(hoveredUnit.transform.position);
        
        if(!unit.CanPlay())
            return; // Can't play
        if(!unit.actionsHolder.CanUse(this))
            return; // Can't do this action
        if(!IsTileInRange(hoveredUnit.tile))
            return; // Enemy is not visible or not in range
        if(!unit.weaponHolder.weapon.hasAvailableAmmoToSpend)
            return; // Out of ammo
        
        OnAttackableEnemyHovered?.Invoke(this, hoveredUnit);
        EnterLean(hoveredUnit);
    }
    
    protected override void OnExitCharacter(Unit exitedUnit)
    {
        if(!unit.CanPlay())
            return; // Can't play
        if(!unit.actionsHolder.CanUse(this))
            return; // Can't do this action

        OnUnitExit?.Invoke(this, exitedUnit);
        ExitLean();
    }
    
    protected override void OnClickAnyCharacter(Unit clickedUnit)
    {
        if(!unit.CanPlay())
            return; // Can't play
        if(!unit.actionsHolder.CanUse(this))
            return; // Can't do this action
        if(unit.team.IsAllyOf(clickedUnit)) 
            return; // Same team
        if(!IsTileInRange(clickedUnit.tile))
            return; // Enemy is not visible or not in range
        if(!unit.weaponHolder.weapon.hasAvailableAmmoToSpend)
            return; // Out of ammo
        
        // Attack
        Attack(clickedUnit);
    }
}
