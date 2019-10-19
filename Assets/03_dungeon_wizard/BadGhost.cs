using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class BadGhost : DWMazeBody
{
    public float speed = 20;
    public float animSpeed = .03f;
    public navdi3.SpriteLot sprs;
    float anim = 0;

    int stuckFrames = 0;
    
    Rigidbody2D body {  get { return GetComponent<Rigidbody2D>(); } }
    SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }
    private void FixedUpdate()
    {
        if (body.velocity.magnitude < speed * 0.95f)
        {
            stuckFrames++;
            if (stuckFrames > 60)
            {
                lastMove = -lastMove;
                my_cell_pos = new twin(master.grid.WorldToCell(this.transform.position));
                TryMove(lastMove);
                stuckFrames = 0;
            }
        } else
        {
            stuckFrames = 0;
        }

        anim += animSpeed;
        anim %= 2;
        spriter.sprite = sprs[11+(int)anim];

        if (IsWithinDistOfCentered(2f))
        {
            TryDWMove();
        }

        body.velocity = ((Vector2)this.ToCentered()).normalized * speed; // go! go! go!
        if (Mathf.Abs(body.velocity.x) > speed * 0.5f) spriter.flipX = body.velocity.x < 0;
    }
}
