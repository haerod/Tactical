using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;
using static Utils;

public class A_Attack : A__Action
{
    [Header("- PARAMETERS -")]
    
    [SerializeField] private int precision = 100;
    
    private Action onAttackDone;
    
    public event EventHandler OnAttackStart;
    public event EventHandler OnAttackExecute;
    public event EventHandler OnAttackEnd;
    public event EventHandler<Unit> OnAttackMiss;
    public event EventHandler<Unit> OnAttackableEnemyHovered;
    public event EventHandler<Unit> OnUnitExit;
    
    private Unit attackTarget;
    
    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================
    
    private void Start()
    {
        OnAttackExecute += Attack_OnAttackExecute;
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        OnAttackExecute -= Attack_OnAttackExecute;
    }
    
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
    public void StartAttack(Unit currentTarget)
    {
        if (!unit.look.CanSee(currentTarget)) 
            return; // Enemy not in sight
        
        attackTarget = currentTarget;
        
        unit.move.OrientTo(currentTarget.transform.position);
        currentTarget.move.OrientTo(unit.transform.position);
        
        StartAction();
        GameEvents.InvokeOnAnyAttackStart(unit);
        OnAttackStart?.Invoke(this, EventArgs.Empty);
    }
    
    public void ExecuteAttack()
    {
        OnAttackExecute?.Invoke(this, EventArgs.Empty);
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
    private void SetOnAttackDone(bool success, int damages, Unit target, Ammo ammo = null)
    {
        onAttackDone = () =>
        {
            if (success)
                target.health.AddDamages(damages, unit.weaponHolder.weaponData.damageType);
            else
            {
                OnAttackMiss?.Invoke(this, target);
                target.anim.StartDodge();
            }
            
            Wait(0.5f, () =>
            {
                GameEvents.InvokeOnAnyAttackEnd(unit);
                OnAttackEnd?.Invoke(this, EventArgs.Empty);
                
                EndAction();
            });
            
            if(ammo)
                ammo.OnProjectileHit -= Ammo_OnProjectileHit;
        };
    }
    
    /// <summary>
    /// Ends the attack.
    /// Called by C_AnimatorScripts, after the shoot animation.
    /// </summary>
    private void EndAttack()
    {
        _camera.Shake();
        onAttackDone();
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
    
    protected override bool IsAvailable()
    {
        Weapon weapon = unit.weaponHolder.weapon;
        
        if (weapon.data.usesAmmo)
            if(!weapon.hasAvailableAmmoToSpend)
                return false; // Not enough ammo
        
        return base.IsAvailable();
    }
    
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
        StartAttack(clickedUnit);
    }
    
    private void Ammo_OnProjectileHit(object sender, EventArgs e)
    {
        EndAttack();
    }
    
    private void Attack_OnAttackExecute(object sender, EventArgs e)
    {
        Unit target = attackTarget;
        Weapon weapon = unit.weaponHolder.weapon;
        int damages = unit.weaponHolder.weaponData.RandomDamages();
        bool isTouching = DiceRoll(100) <= GetChanceToTouch(target);
        
        if (weapon.data.useProjectile)
        {
            //Attack attack = new Attack(target, damages, weapon.data.damageType);
            Ammo ammo = Instantiate(
                    weapon.data.ammo,
                    unit.graphics.rightHand.position, 
                    Quaternion.identity)
                .GetComponent<Ammo>();
            ammo.SetGraphicsActive(true);
            ammo.OnProjectileHit += Ammo_OnProjectileHit;

            if (isTouching)
            {
                SetOnAttackDone(true, damages, target, ammo);
                ammo.transform.LookAt(target.graphics.torso.position);
                ammo.GoTo(target.graphics.torso);
                SetOnAttackDone(true, damages, target);
            }
            else
            {
                SetOnAttackDone(false, 0, target, ammo);
                ammo.transform.LookAt(target.tile.worldPosition);
                ammo.GoTo(target.tile.transform);
                SetOnAttackDone(false, 0, target);
            }
        }
        else
        {
            if(isTouching)
                SetOnAttackDone(true, damages, target);
            else
                SetOnAttackDone(false, 0, target);
            
            EndAttack();
        }
    }
}
