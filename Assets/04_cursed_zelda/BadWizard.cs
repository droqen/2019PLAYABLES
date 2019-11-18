using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class BadWizard : MazeBody
{
    public float moveSpeed = 30;

    // do nothing special.
    private void FixedUpdate()
    {
        if (IsWithinDistOfCentered(2f))
        {
            // dope
            var dirs = new ChoiceStack<twin>(twin.compass);
            dirs.RemoveAll(-lastMove);
            lastMove = dirs.GetFirstTrue(TryMove);
        } else
        {
            GetComponent<Rigidbody2D>().velocity = moveSpeed * ToCentered().normalized;
        }
    }
}
