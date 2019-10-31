using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class CommMazer : MazeBody
{
    twin target = new twin(1,1);
    twin flip_target = new twin(17,16);

    int stuck = 0;
    int patience = 0;

    public float speed = 30;
    private void FixedUpdate()
    {
        if (IsWithinDistOfCentered(2f))
        {
            if (my_cell_pos == target) ReachedTarget();

            ChooseNextPosition();
        }
        else
        {
            if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < speed * speed * 0.25f) StuckFrame();

            var dir = ToCentered().normalized;
            GetComponent<Rigidbody2D>().velocity = dir * speed;
            if (dir.x > 0.5f) GetComponent<SpriteRenderer>().flipX = false;
            if (dir.x < -0.5f) GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void ReachedTarget()
    {
        var t = target;
        target = flip_target;
        flip_target = t;
    }

    void ChooseNextPosition()
    {
        bool smartMoved = false;
        var smartNext = commuterxxi.Instance.GetAntNextStep(this, target);
        if (smartNext.HasValue)
        {
            var smartDir = smartNext.Value - my_cell_pos;
            if (smartDir != -lastMove && TryMove(smartDir))
            {
                smartMoved = true;
            }
        }

        if (!smartMoved)
        {
            // did not move smartly? move randomly (like a pac-man ghost without any AI)
            ChoiceStack<twin> dirs = new ChoiceStack<twin>(twin.compass);
            dirs.RemoveAll(-lastMove);
            lastMove = dirs.GetFirstTrue(TryMove);
        }
    }
    void StuckFrame()
    {
        stuck++;
        if (stuck > patience)
        {
            patience = Random.Range(10, 30);
            my_cell_pos = new twin(master.grid.WorldToCell(transform.position));
            lastMove = -lastMove;
            TryMove(lastMove);
        }
    }
}
