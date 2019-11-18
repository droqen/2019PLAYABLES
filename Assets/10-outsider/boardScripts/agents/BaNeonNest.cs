namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaNeonNest : BoardAgent
    {
        public int food = 0;

        public int eggs = 5;

        public int incubation = 0;

        public void Setup(Board board, twin cell)
        {
            base.BoardSetup(board, cell);
        }

        public void ReceiveFood()
        {
            this.food++;
            if (this.food % 2 == 0) this.eggs++;
        }

        private void FixedUpdate()
        {
            SetVelocityApproachTarget();
            if (eggs > 0)
            {
                incubation++;
                if (incubation > 50)
                {
                    incubation = Random.Range(-25, 25);
                    eggs--;
                    xxi.banks["neon ant"].Spawn<BaNeonAnt>(xxi.GetEntLot("ants")).Setup(board, this);
                }
            }
        }
    }

}