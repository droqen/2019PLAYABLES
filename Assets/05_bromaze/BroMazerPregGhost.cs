using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class BroMazerPregGhost : BroMazer
{
    public bool pregnant = true;

    private void FixedUpdate()
    {
        SpriterUpdate();
        MoveUpdate_Center(pregnant? baseSpeed * 0.5f : baseSpeed);
        if (CheckIsFrustrated(pregnant?120:60))
        {
            if (pregnant)
            {
                // split into 2 babies
                lastMove = new twin(this.lastMove.y, this.lastMove.x);
                RefreshCell();

                this.BecomeBaby();

                for (int i = 0; i < 2; i++)
                {
                    var baby = bromazexxi.Instance.banks["pregghost"].Spawn<BroMazerPregGhost>(bromazexxi.Instance.CreaturesLot);
                    baby.Setup(master, my_cell_pos);

                    baby.lastMove = -this.lastMove;

                    baby.BecomeBaby();
                }
            } else
            {
                // just pick a new dir. try turning around.
                lastMove = -lastMove;
                RefreshCell();
                TryMove(lastMove);
            }
        }
        if (IsWithinDistOfCentered(2f))
        {
            var dirs = new ChoiceStack<twin>();
            dirs.AddManyThenLock(twin.compass);
            dirs.RemoveAll(-lastMove);
            lastMove = dirs.GetFirstTrue(this.TryMove);
        }
    }

    void BecomeBaby()
    {
        if (this.pregnant)
        {
            this.pregnant = false;
            for (var i = 0; i < this.bani.spriteIds.Length; i++)
            {
                this.bani.spriteIds[i] += 10;
            }
            TryMove(lastMove);
        }
    }
}
