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
    public static event EventHandler<U__Unit> OnAnyAttackStart;
    public event EventHandler OnAttackEnd;
    public event EventHandler<U__Unit> OnAttackMiss;

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
            .Where(chara => IsTileInRange(chara.tile))
            .Select(chara => chara.tile)
            .ToList();
    
    /// <summary>
    /// Attacks the target and starts an action in the end.
    /// </summary>
    /// <param name="currentTarget"></param>
    public void Attack(U__Unit currentTarget)
    {
        unit.SetCanPlayValue(false);
        
        if (!unit.look.CanSee(currentTarget)) 
            return; // Enemy not in sight

        U__Unit target = currentTarget;

        unit.move.OrientTo(target.transform.position);
        target.move.OrientTo(unit.transform.position);

        int damages = unit.weaponHolder.GetCurrentWeapon().GetDamages();

        OnAttackStart?.Invoke(this, EventArgs.Empty);
        OnAnyAttackStart?.Invoke(this, unit);

        unit.anim.StartAttack();
        
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

        // Muzzle flash

        GameObject muzzleFlash = unit.weaponHolder.GetCurrentWeaponGraphics().GetMuzzleFlash();

        if (!muzzleFlash) 
            return; // No muzzle flash
        
        muzzleFlash.SetActive(true);
        Wait(0.2f, () => muzzleFlash.SetActive(false));
    }
    
    /// <summary>
    /// Returns the percent of chance to touch the target, including the reduction by distance.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public int GetChanceToTouch(U__Unit target)
    {
        int precisionToReturn = precision;
        Weapon currentWeapon = unit.weaponHolder.GetCurrentWeapon();
        int targetProtectionValue = target.cover.GetCoverProtectionValueFrom(unit);
        
        if(!currentWeapon.IsMeleeWeapon())
            precisionToReturn -= targetProtectionValue;
        
        for (int i = 0; i < currentWeapon.GetRange(); i++)
            precisionToReturn -= unit.weaponHolder.GetCurrentWeapon().GetPrecisionMalusByDistance();
        
        return precisionToReturn < 0 ? 0 : precisionToReturn;
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
    private void SetOnAttackDone(bool success, int damages, U__Unit target)
    {
        onAttackDone = () =>
        {
            if (success)
            {
                target.health.AddDamages(damages, unit.weaponHolder.GetCurrentWeapon().GetDamageTypes());
            }
            else
            {
                OnAttackMiss?.Invoke(this, target);
                target.anim.StartDodge();
            }
            
            Wait(0.5f, () =>
            {
                OnAttackEnd?.Invoke(this, EventArgs.Empty);
                _units.EndCurrentUnitTurn();
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
        Weapon currentWeapon = unit.weaponHolder.GetCurrentWeapon();
        List<Tile> los = unit.look.GetTilesOfLineOfSightOn(tile.coordinates);

        if (currentWeapon.GetTouchInView())
            return unit.look.CanSee(tile.character);
        if (currentWeapon.IsMeleeWeapon())
            return los.Count == 0;
        else
            return los.Count < currentWeapon.GetRange();
    }
    
    private void EnterLean(U__Unit targetUnit)
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
    
    private void ExitLean()
    {
        // transform.position = unit.coordinates.ToVector3();
    }
    
    // ======================================================================
    // ACTION OVERRIDE METHODS
    // ======================================================================
    
    protected override void OnHoverEnemy(U__Unit hoveredUnit)
    {
        unit.move.OrientTo(hoveredUnit.transform.position);
        
        if(!unit.CanPlay())
            return; // Can't play
        
        if(!IsTileInRange(hoveredUnit.tile))
            return; // Enemy is not visible or not in range
        
        EnterLean(hoveredUnit);
        unit.anim.StartAim();
    }
    
    protected override void OnExitCharacter(U__Unit exitedUnit)
    {
        if(!unit.CanPlay())
            return; // Can't play
        
        ExitLean();
        unit.anim.StopAim();
    }
    
    protected override void OnClickAnyCharacter(U__Unit clickedUnit)
    {
        if(!unit.CanPlay())
            return; // Can't play
        
        if(unit.team.IsAllyOf(clickedUnit)) 
            return; // Same team

        if(!IsTileInRange(clickedUnit.tile))
            return; // Enemy is not visible or not in range
        
        // Attack
        Attack(clickedUnit);
    }
}
