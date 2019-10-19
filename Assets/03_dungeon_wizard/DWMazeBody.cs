using UnityEngine;
using System.Collections;

using navdi3;

public class DWMazeBody : navdi3.maze.MazeBody
{
    public void TryDWMove()
    {
        twin.StraightenCompass();
        ChoiceStack<twin> directions = new ChoiceStack<twin>();
        if (lastMove.taxicabLength == 2) directions.AddManyThenLock(
            new twin(lastMove.x,0),
            new twin(0,lastMove.y),
            lastMove); // prefer to continue in your weird diagonal direction.

        bool open_space = false;
        for(int i = 0; i < 4; i++)
        {
            var dira = twin.compass[i];
            var dirb = twin.compass[(i + 1) % 4];
            if ((dira == lastMove || dirb == lastMove)
                && CanMoveTo(my_cell_pos + dira)
                && CanMoveTo(my_cell_pos + dirb)
                && CanMoveTo(my_cell_pos + dira + dirb))
            {
                directions.Add(dira + dirb);
                open_space = true; // moving through an open space.
            }
        }
        if (open_space) directions.Add(lastMove); // in an open space, i can continue in the same direction.
        directions.Lock();

        directions.AddManyThenLock(twin.compass);

        directions.RemoveAll(-lastMove);

        lastMove = directions.GetFirstTrue(TryMove);
    }
}
