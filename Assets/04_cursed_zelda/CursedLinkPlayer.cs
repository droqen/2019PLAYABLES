using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class CursedLinkPlayer : navdi3.maze.MazeBodyKinematic
{
    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }
    BitsyAni ani { get { return GetComponent<BitsyAni>(); } }

    public navdi3.twin facing;

    public float speed = 15;
    public float stillAnispd = .02f;
    public float movingAnispd = .05f;

    override public void FixedUpdate()
    {
        base.FixedUpdate();

        ani.speed = navdi3.Util.remap(0, speed, stillAnispd, movingAnispd, body.velocity.magnitude);

        var pinMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        pinMove.x += navdi3.nin.JoyAxis(1, 4);
        pinMove.y -= navdi3.nin.JoyAxis(1, 5);
        pinMove.Normalize();
        body.velocity = body.velocity * 0.5f + 0.5f * pinMove * speed;

        if (pinMove != Vector2.zero) facing = new navdi3.twin(pinMove);
        if (facing.x != 0 && facing.y != 0) facing.y = 0;
    }
}
