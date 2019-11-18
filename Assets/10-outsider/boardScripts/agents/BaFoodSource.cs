namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaFoodSource : BoardAgent
    {
        public int food = 10;

        public void Setup(Board board, twin cell)
        {
            base.BoardSetup(board, cell);
        }

        private void FixedUpdate()
        {
            SetVelocityApproachTarget();
        }

        public void TakeFood()
        {
            food--;
            if (food <= 0) Object.Destroy(gameObject);
        }
    }

}