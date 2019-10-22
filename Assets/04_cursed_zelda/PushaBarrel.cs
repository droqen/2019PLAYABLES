using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class PushaBarrel : MazeBody
{
    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    private void Start()
    {
        var annie = GetComponent<BitsyAni>();
        annie.speed *= Random.Range(.5f, 1.5f);
        annie.anim = Random.value * float.MaxValue;
    }
    void FixedUpdate()
    {
        my_cell_pos = new twin(master.grid.WorldToCell(transform.position));
        if (IsWithinDistOfCentered(1f)) // close enough
        {
            body.velocity *= 0.8f;
        } else
        {
            body.velocity *= 0.9f;
            body.velocity += (Vector2)ToCentered().normalized * 1;
        }
    }
}
