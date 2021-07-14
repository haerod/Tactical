using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static M__Managers;

public class Attack : MonoBehaviour
{
    public Vector2Int damagesRange = new Vector2Int(3, 5);
    public Character target;

    [Header("REFERENCES")]
        
    [SerializeField] private Character c = null;
    private Action OnAttackDone;

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
        target = currentTarget;
        c.move.ClearAreaZone();
        c.move.OrientTo(target.transform.position);
        target.move.OrientTo(c.transform.position);
        int damages = UnityEngine.Random.Range(damagesRange.x, damagesRange.y + 1);
        _inputs.ClearFeedbacksAndValues();
        _inputs.SetClick(false);
        c.anim.StartShoot();        
        OnAttackDone = () => 
        {
            target.health.AddDamages(damages, c);
            Wait(.5f, () => 
            {
                _inputs.SetClick();
                c.move.EnableMoveArea();
                OnEnd();
            });
        };
    }

    public void EndAttack()
    {
        _camera.Shake();
        OnAttackDone();
    }

    public Character ClosestCharacterOnSight()
    {
        return _characters.characters
            .Where(o => o != c) // remove emitter
            .Where(o => HasSightOn(o)) // get all enemies on sight
            .OrderBy(o => LineOfSight(o.Tile()).Count()) // order enemies by distance
            .FirstOrDefault(); // return the lowest
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
