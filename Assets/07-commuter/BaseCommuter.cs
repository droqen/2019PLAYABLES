using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using navdi3;
using navdi3.maze;

abstract public class BaseCommuter : MazeBody
{
    /// inspector
    /// 
    public float move_speed = 30f;
    public bool no_turn_around = true;
    public int patience = 30;



    /// properties
    /// 
    protected int stuckFrames = 0;

    Rigidbody2D _Body;
    public Rigidbody2D Body { get { if (_Body == null) _Body = GetComponent<Rigidbody2D>(); return _Body; } }
    SpriteRenderer _Spriter;
    public SpriteRenderer Spriter { get { if (_Spriter == null) _Spriter = GetComponent<SpriteRenderer>(); return _Spriter; } }



    protected AStarPather pather;
    public void Setup(MazeMaster master, twin cell_pos, AStarPather pather)
    {
        this.pather = pather;
        base.Setup(master, cell_pos);
    }



    public void RecenterCellPos()
    {
        my_cell_pos = new twin(master.grid.WorldToCell(transform.position));
    }



    public bool TryVelocityUpdate()
    {
        if (Body.velocity.sqrMagnitude < move_speed * move_speed * .25f)
        {
            stuckFrames ++;
            if (stuckFrames > patience) return false; // i give up!
        }
        else stuckFrames = 0;

        var dir = ToCentered().normalized; // never give up! always keep trying!
        Body.velocity = dir * move_speed;
        return true;
    }



    public void UpdateSpriteFlipX()
    {
        GetComponent<SpriteRenderer>().flipX &= Body.velocity.x <= 0.5f * move_speed;
        GetComponent<SpriteRenderer>().flipX |= Body.velocity.x < -0.5f * move_speed;
    }



    public void GetPathTo(twin target, out twin move, out AStarPath path)
    {
        move = twin.zero;
        path = new AStarPath(pather, my_cell_pos, target);
        if (path.cells == null) return;
        foreach (var cell in path.cells) if (cell != my_cell_pos)
            {
                move = cell - my_cell_pos;
                return;
            }
    }



    // always try this first!
    public bool TryTakeForcedMove()
    {
        twin? onlyAvailableMove = null;
        foreach (var dir in twin.compass)
        {
            if ((no_turn_around? dir != -lastMove: true)
                && CanMoveTo(my_cell_pos + dir))
            {
                if (onlyAvailableMove.HasValue) return false; // 2+ moves available
                else onlyAvailableMove = dir;
            }
        }

        if (!onlyAvailableMove.HasValue) return false; // 0 moves available

        // exactly 1 move available:
        TryMove(onlyAvailableMove.Value);
        return true;
    }



    public bool TryTakeNextMoveTo(twin target)
    {
        GetPathTo(target, out var move, out var path); // path is unused
        if (move == twin.zero || (no_turn_around && move == -lastMove)) return false; // failed
        return TryMove(move);
    }



    public void TakeAnyMove()
    {
        var dirs = new ChoiceStack<twin>(twin.compass);
        if (no_turn_around) dirs.RemoveAll(-lastMove);
        lastMove = dirs.GetFirstTrue(TryMove);
    }
}
