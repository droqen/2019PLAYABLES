using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class BadPigeon : DWMazeBody
{
    public float speed = 20;
    public float animSpeed = .03f;
    public float flyingSpeed = 40;
    public float flyingAnimSpeed = .03f;
    public navdi3.SpriteLot sprs;
    float anim = 0;
    bool flying = false;
    twin flyingToCell;

    int stuckFrames = 0;

    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }
    private void FixedUpdate()
    {
        var currentSpeed = speed;

        if (flying)
        {
            anim += flyingAnimSpeed;
            anim %= 2;
            spriter.sprite = sprs[23 + (int)anim];

            if (IsWithinDistOfCentered(2f))
            {
                StopFlying();
            }

            currentSpeed = flyingSpeed;
        }
        else
        {

            anim += animSpeed;
            anim %= 2;
            spriter.sprite = sprs[21 + (int)anim];

            if (IsWithinDistOfCentered(2f))
            {
                TryDWMove();
            }
        }

        if (body.velocity.magnitude < currentSpeed * 0.95f)
        {
            stuckFrames++;
            if (stuckFrames > 10) // they're skittish
            {
                my_cell_pos = new twin(master.grid.WorldToCell(this.transform.position));
                stuckFrames = 0;
                StartFlying(dungeonxxi.Instance.GetRandomFreeCell(cell => { return (cell - my_cell_pos).taxicabLength > 3; }));
            }
        }
        else
        {
            stuckFrames = 0;
        }

        body.velocity = ((Vector2)this.ToCentered()).normalized * currentSpeed; // go! go! go!
        if (Mathf.Abs(body.velocity.x) > currentSpeed * 0.5f) spriter.flipX = body.velocity.x < 0;
    }

    void StartFlying(twin? target)
    {
        if (target.HasValue)
        {
            my_cell_pos = target.Value;
            flying = true;
            flyingToCell = target.Value;
            GetComponent<Collider2D>().isTrigger = true;
        }
    }
    void StopFlying()
    {
        my_cell_pos = new twin(master.grid.WorldToCell(this.transform.position));
        flying = false;
        GetComponent<Collider2D>().isTrigger = false;
    }
}
