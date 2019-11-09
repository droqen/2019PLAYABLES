using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class CommuterPlayer : BaseCommuter
{
    private void FixedUpdate()
    {
        var pinmove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Body.velocity = pinmove.normalized * move_speed;
        this.UpdateSpriteFlipX();

        GetComponent<BitsyAni>().speed = pinmove.sqrMagnitude * .12f + .05f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTouchedAnt(collision.gameObject.GetComponent<CommuterAnt>());
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnTouchedAnt(collision.gameObject.GetComponent<CommuterAnt>());
    }
    void OnTouchedAnt(CommuterAnt ant)
    {
        if (ant == null) return;
        if (!ant.hasBerry) return;

        ant.LoseBerry();
    }
}
