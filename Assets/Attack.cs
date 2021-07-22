using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class Attack : MonoBehaviour
{
    public int actionPointsCost = 3;
    public Vector2Int damagesRange = new Vector2Int(3, 5);
    public Character target;

    [Header("REFERENCES")]
        
    [SerializeField] private Character c = null;
    [SerializeField] private GameObject muzzleFlare = null;

    private Action OnAttackDone;
    private List<Tile> attackTiles = new List<Tile>();

    // ======================================================================
    // MONOBEHAVIOUR
    // ======================================================================

    // ======================================================================
    // PUBLIC METHODS
    // ======================================================================

    public bool HasSightOn(Character target)
    {
        if (AreObstacles(LineOfSight(target.Tile())))
            return false; // Exit : obstacles

        return true; // Exit : has sight on target
    }

    public void AttackTarget(Character currentTarget, Action OnEnd)
    {
        if (c.actionPoints.actionPoints < actionPointsCost)
        {
            OnEnd();
            return; // EXIT : No action points aviable
        }

        target = currentTarget;

        c.ClearTilesFeedbacks();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);

        int damages = UnityEngine.Random.Range(damagesRange.x, damagesRange.y + 1);

        _inputs.ClearFeedbacksAndValues();
        _inputs.SetClick(false);

        _ui.SetActionPlayerUIActive(false);

        c.anim.StartShoot();        
        c.actionPoints.RemoveActionPoints(actionPointsCost);

        OnAttackDone = () => 
        {
            target.health.AddDamages(damages, c);
            Wait(0.5f, () => 
            {
                _inputs.SetClick();
                c.EnableTilesFeedbacks();

                OnEnd();
            });
        };
    }

    public void EndAttack()
    {
        _camera.Shake();
        _ui.SetActionPlayerUIActive(true);
        OnAttackDone();

        // Muzzle flare
        muzzleFlare.SetActive(true);
        Wait(0.1f, () => muzzleFlare.SetActive(false));
    }

    public Character ClosestCharacterOnSight()
    {
        return _characters.characters
            .Where(o => o != c) // remove emitter
            .Where(o => HasSightOn(o)) // get all enemies on sight
            .OrderBy(o => LineOfSight(o.Tile()).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
    }

    public void EnableAttackTiles()
    {
        if(c.actionPoints.actionPoints < c.attack.actionPointsCost)
        {
            return;
        }

        foreach (Character character in _characters.characters)
        {
            if (character == c) continue;
            if (!HasSightOn(character)) continue;

            attackTiles.Add(character.Tile());
        }

        foreach (Tile t in attackTiles)
        {
            t.SetMaterial(Tile.TileMaterial.Range);
        }
    }

    public void ClearAttackTiles()
    {
        foreach (Tile t in attackTiles)
        {
            t.SetMaterial(Tile.TileMaterial.Basic);
        }

        attackTiles.Clear();
    }

    // ======================================================================
    // PRIVATE METHODS
    // ======================================================================

    private List<Tile> LineOfSight(Tile targetTile)
    {
        return _pathfinding.LineOfSight(c.Tile(), targetTile).ToList();
    }

    private bool AreObstacles(List<Tile> lineOfSight)
    {
        if (Utils.IsVoidList(lineOfSight)) return false;

        foreach (Tile t in lineOfSight)
        {
            if (t.Character()) return true;
        }

        return false;
    }

    private void Wait(float time, Action OnEnd)
    {
        StartCoroutine(Wait_Co(time, OnEnd));
    }

    IEnumerator Wait_Co(float time, Action OnEnd)
    {
        yield return new WaitForSeconds(time);

        OnEnd();
    }
}
