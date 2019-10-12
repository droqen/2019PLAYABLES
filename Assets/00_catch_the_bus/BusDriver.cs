using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class BusDriver : MonoBehaviour
{
    public Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }

    public Sprite[] doorSprites;
    public SpriteRenderer doorSpriter;
    public Transform doorTransform;

    public float damping = 0.95f;
    public float acceleration = 1.00f;

    float door_open = 0.0f;

    public bool IsEnterable { get { return door_open > 0.9f; } }
    public bool IsDangerous { get { return Mathf.Abs(body.velocity.x)>7; } }
    public bool IsLethal { get { return Mathf.Abs(body.velocity.x)>10; } }

    private void Start()
    {
        doorSpriter.enabled = false;
    }

    private void Update()
    {
        var py = Input.GetAxisRaw("Vertical");
        if (py < -0.1f || py > 0.1f)
        {
            door_open = Mathf.Clamp01(door_open + py * Time.deltaTime);
            if (door_open < 0.01f) doorSpriter.enabled = false;
            else
            {
                doorSpriter.enabled = true;
                doorSpriter.sprite = doorSprites[Mathf.FloorToInt(door_open * 2.1f)];
            }
        }

        var vx = body.velocity.x;
        var px = Input.GetAxisRaw("Horizontal");
        if (px < -0.1f || px > 0.1f)
        {
            vx += px * Time.deltaTime * acceleration;
        }
        vx *= damping;
        body.velocity += Vector2.right * (vx - body.velocity.x);
    }

}
