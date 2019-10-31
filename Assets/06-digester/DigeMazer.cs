using UnityEngine;
using System.Collections;

using navdi3;
using navdi3.maze;

public class DigeMazer : MazeBody
{
    public float speed = 30f;

    int patience = 60;
    int framesStuckFor = 0;

    private void Start()
    {
        patience = Random.Range(10, 50);
    }

    virtual protected void FixedUpdate()
    {

        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < speed * speed * 0.5f) // being slowed down?
        {
            framesStuckFor++;
            if (framesStuckFor>=patience)
            {
                // react!
                my_cell_pos = new twin(master.grid.WorldToCell(transform.position));
                lastMove = -lastMove;
                ChooseMove_no180();
                //TryMove(-lastMove); // move backwards, reset pos.
                patience = Random.Range(10, 50);
            }
        } else
        {
            framesStuckFor = 0;
        }

        if (IsWithinDistOfCentered(2f))
        {
            // choose new dir
            ChooseMove_no180();
        } else
        {
            GetComponent<Rigidbody2D>().velocity = speed * ((Vector2)ToCentered()).normalized;
        }
    }

    protected void ChooseMove_no180()
    {
        var dirs = new ChoiceStack<twin>(); dirs.AddManyThenLock(twin.compass);
        dirs.RemoveAll(-lastMove);
        lastMove = dirs.GetFirstTrue(TryMove);

        if (lastMove.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = lastMove.x < 0;
        }
    }
}
