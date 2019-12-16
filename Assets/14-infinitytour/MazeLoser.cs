using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class MazeLoser : MazeBody
{
    infinitytourxxi xxi { get { return infinitytourxxi.Instance; } }

    public float speed = 30f;
    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    private void FixedUpdate()
    {
        if (IsWithinDistOfCentered(2f))
        {
            var dirs = new ChoiceStack<twin>(twin.compass);
            dirs.RemoveAll(-lastMove);
            lastMove = dirs.GetFirstTrue(TryMove);
        } else
        {
            body.velocity = body.velocity * .5f + .5f * (Vector2)ToCentered() * speed;
        }
    }

    override public bool CanMoveTo(twin target_pos) { return xxi.Gett(target_pos) == 0; }
}
