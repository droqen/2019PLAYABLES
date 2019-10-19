using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyCtrl : MonoBehaviour
{
    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }

    public navdi3.twin facing;

    public float speed = 15;
    public float animSpeed = .01f;
    public float movingAnimSpeed = .05f;
    public navdi3.SpriteLot sprs;
    float anim = 0;

    void FixedUpdate()
    {
        anim += navdi3.Util.remap(0, speed, animSpeed, movingAnimSpeed, body.velocity.magnitude);
        anim %= 4;
        switch((int)anim)
        {
            case 0: spriter.sprite = sprs[0]; break;
            case 1: spriter.sprite = sprs[1]; spriter.flipX = true; break;
            case 2: spriter.sprite = sprs[0]; break;
            case 3: spriter.sprite = sprs[1]; spriter.flipX = false; break;
        }

        var pinMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        pinMove.x += navdi3.nin.JoyAxis(1, 4);
        pinMove.y -= navdi3.nin.JoyAxis(1, 5);
        body.velocity = pinMove * speed;

        if (pinMove != Vector2.zero) facing = new navdi3.twin(pinMove);
        if (facing.x != 0 && facing.y != 0) facing.y = 0;
    }
}
