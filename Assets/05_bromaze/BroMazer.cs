using UnityEngine;
using System.Collections;

using navdi3;
using navdi3.maze;

[RequireComponent(typeof(BitsyAni))]
public class BroMazer : MazeBody
{
    public Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    public SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }
    public BitsyAni bani { get { return GetComponent<BitsyAni>(); } }

    public float baseSpeed = 20;
    public float moveAccelMult = .25f;
    public float moveAccelLinear = .1f;

    public bool autoFlipX = true;
    public float standingAniSpeed = .01f;
    public float movingAniSpeed = .05f;

    Vector2 lastDesiredVelocity;
    
    protected void SpriterUpdate()
    {
        bani.speed = Util.remap(0, baseSpeed, standingAniSpeed, movingAniSpeed, body.velocity.magnitude);
        if (Mathf.Abs(lastDesiredVelocity.x) > 0.1f * baseSpeed)
            spriter.flipX = lastDesiredVelocity.x < 0;
    }

    protected void MoveUpdate_TargetVelocity(Vector2 targetVelocity)
    {
        lastDesiredVelocity = targetVelocity;
        
        if (moveAccelMult > 0)
        {
            body.velocity = body.velocity * (1 - moveAccelMult) + (moveAccelMult) * targetVelocity;
        }
        if (moveAccelLinear > 0)
        {
            var toTarget = targetVelocity - body.velocity;
            if (toTarget.sqrMagnitude > Mathf.Pow(moveAccelLinear, 2))
            {
                body.velocity += toTarget.normalized * moveAccelLinear;
            } else
            {
                body.velocity = targetVelocity;
            }
        }
    }

    public int moveCenterStuckFrames = 0;
    Vector2 lastCenterVelocity;

    protected void MoveUpdate_Center(float speed)
    {
        if (body.velocity.sqrMagnitude < lastCenterVelocity.sqrMagnitude
            || body.velocity.x * ToCentered().x < 0
            || body.velocity.y * ToCentered().y < 0
            )
        {
            moveCenterStuckFrames++;
        }
        else
        {
            moveCenterStuckFrames = 0;
        }
        
        this.MoveUpdate_TargetVelocity(((Vector2)ToCentered()).normalized * speed);
        lastCenterVelocity = body.velocity;
    }

    protected bool CheckIsFrustrated(int willpower)
    {
        if (moveCenterStuckFrames > willpower)
        {
            moveCenterStuckFrames = 0;
            return true;
        } else
        {
            return false;
        }
    }

    protected void RefreshCell()
    {
        my_cell_pos = new twin(master.grid.WorldToCell(this.transform.position));
    }

    protected void ChooseNewDir_Forward()
    {
        var dirs = new ChoiceStack<twin>();
        dirs.Add(twin.down); // prefer 'down': gravity
        dirs.AddManyThenLock(twin.compass);
        dirs.RemoveAll(-lastMove);
        lastMove = dirs.GetFirstTrue(this.TryMove);
    }
    protected void ChooseNewDir_Reverse()
    {
        lastMove = -lastMove;
        RefreshCell();
        TryMove(lastMove);
    }
}
