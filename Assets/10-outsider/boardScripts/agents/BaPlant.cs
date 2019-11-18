namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaPlant : BoardAgent
    {
        public int cycleLength = 50;
        public float collisionDamage = 0.5f;

        public Color nutrientColour = Color.cyan;

        float thriving = 0.5f;
        float heart = 0.5f;
        float mature = 0.0f;
        int toNextCycle = 0;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var agent = collision.gameObject.GetComponent<BoardAgent>();
            if (agent != null && !(agent is BaPlant))
            {
                // did i hit a boardagent that's not a plant?
                // if yes: take damage.
                heart -= Random.value * collisionDamage * 2;
            }
        }

        public void Setup(Board board, twin cell)
        {
            base.BoardSetup(board, cell);
            this.thriving = 0.5f;
            this.heart = 1.0f;
            this.mature = 0.0f;
            this.toNextCycle = this.cycleLength;

            transform.localScale = Vector3.one * 0.5f;
        }

        public void Spread()
        {
            this.heart -= 0.5f;
            xxi.banks[this.gameObject.name].Spawn<BaPlant>(xxi.GetEntLot("plants")).Setup(board, my_cell_pos);
        }

        private void FixedUpdate()
        {

            if (this.mature < 1)
            {
                if (IsWithinDistOfCentered(2f, offset:subtileOffset))
                {
                    this.mature += Random.value * 0.5f;
                    if (this.mature >= 1) transform.localScale = Vector3.one;
                    else
                    {
                        var dirs = new ChoiceStack<twin>(twin.compass);
                        dirs.RemoveAll(-lastMove);
                        lastMove = dirs.GetFirstTrue(TryMove);
                    }
                } else if (bodyStuckFrames > 20)
                {
                    bodyStuckFrames = 0;
                    SnapMyCellPos();
                    TryMove(-lastMove);
                }
            }

            SetVelocityApproachTarget();

            this.toNextCycle--;
            if (this.toNextCycle <= 0)
            {
                // too many plants in one space will choke + die!

                float[] nutrients_need = { 0, 0, 0 };

                HashSet<twin> myCellAndOrthos = new HashSet<twin>();
                myCellAndOrthos.Add(my_cell_pos);
                foreach (var dir in twin.compass) myCellAndOrthos.Add(my_cell_pos + dir);

                foreach(var agent in board.GetAgentsAt(myCellAndOrthos))
                {
                    if (agent is BaPlant)
                    {
                        var plant = (BaPlant)agent;
                        if (plant.mature >= 1)
                        {
                            nutrients_need[0] += plant.nutrientColour.r;
                            nutrients_need[1] += plant.nutrientColour.g;
                            nutrients_need[2] += plant.nutrientColour.b;
                        }
                    }

                    if (agent is BaFoodSource)
                    {
                        this.thriving += 0.25f; // food sources are great for thriving plants :)
                    }
                }

                // if there's a need of 10, and 5 are available, my nutrient share will be 50%. that's simple.
                // if there's a need of 15, and 5 are avail, i'll get 33%. get the reciprocal, simple.
                // then i need to take all those according to how much i need to see how many nutrients i'll have altogether, right?

                // so if i need .75 green and my neighbour does too... we have a combined need of 1.5
                // my thriving will go down by (.75)*(random)*(1-(66%)) // <-- we'll each be getting 66%
                // therefore the 'shortage' is 1-33%
                // my thriving will go down by (.75)*(random*(.33)), which means i'll be losing thriving:
                    // i'll lose 0.12375 on average each frame, too much for my 0.1 to offset.

                if (nutrients_need[0] > 1) this.thriving -= this.nutrientColour.r * Random.value * (1 - (1 / nutrients_need[0]));
                if (nutrients_need[1] > 1) this.thriving -= this.nutrientColour.g * Random.value * (1 - (1 / nutrients_need[1]));
                if (nutrients_need[2] > 1) this.thriving -= this.nutrientColour.b * Random.value * (1 - (1 / nutrients_need[2]));
                this.thriving = Mathf.Clamp01(this.thriving + (mature<1?0:0.1f));

                this.heart = Mathf.Clamp01(this.heart + 0.2f * (this.thriving - 0.5f));

                if (this.heart >= 1 && this.mature >= 1)
                {
                    Spread();
                }

                transform.localScale = Vector3.one;
                GetComponent<SpriteRenderer>().color = Color.white;

                if (mature < 1) transform.localScale *= 0.5f;
                if (this.heart < .4f) GetComponent<SpriteRenderer>().color = Color.grey;
                if (this.heart < .2f) transform.localScale *= 0.5f;

                if (this.heart <= 0)
                {
                    Object.Destroy(this.gameObject);
                } else
                {
                    this.toNextCycle = this.cycleLength;
                }
            }
        }
    }

}