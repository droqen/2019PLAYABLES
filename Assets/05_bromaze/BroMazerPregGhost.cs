using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class BroMazerPregGhost : BroMazer
{
    public bool pregnant = true;
    public int ghostly = 0;

    private void FixedUpdate()
    {
        SpriterUpdate();
        MoveUpdate_Center(pregnant? baseSpeed * 0.5f : baseSpeed);
        if (ghostly > 0)
        {
            ghostly--;
            if (ghostly <= 0) BecomeUnghostly();
        }

        if (CheckIsFrustrated(pregnant?120:30))
        {
            if (pregnant)
            {
                ChooseNewDir_Reverse();
                this.BecomeBaby();
                this.BecomeGhostly();
                transform.position += new twin(Random.Range(-2, 2 + 1), Random.Range(-2, 2 + 1));

                // split into 3 babies
                for (int i = 0; i < 2; i++)
                {
                    var baby = bromazexxi.Instance.banks["pregghost"].Spawn<BroMazerPregGhost>(bromazexxi.Instance.CreaturesLot);
                    baby.Setup(master, my_cell_pos);

                    baby.BecomeBaby();
                    baby.BecomeGhostly();
                    baby.transform.position = this.transform.position + new twin(Random.Range(-2, 2 + 1), Random.Range(-2, 2 + 1));

                    baby.lastMove = -this.lastMove;
                    baby.RefreshCell();
                    baby.ChooseNewDir_Forward();
                }
            } else
            {
                // just pick a new dir. try turning around.
                BecomeGhostly();
            }
        }
        if (IsWithinDistOfCentered(2f))
        {
            ChooseNewDir_Forward();
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
    void BecomeGhostly()
    {
        if (this.ghostly <= 0)
        {
            this.ghostly = Random.Range(30,120);
            this.spriter.color = new Color(1, 1, 1, .5f);
            this.GetComponent<Collider2D>().isTrigger = true;
        }
    }
    void BecomeUnghostly()
    {
        this.ghostly = 0;
        this.spriter.color = Color.white;
        this.GetComponent<Collider2D>().isTrigger = false;
    }
}
