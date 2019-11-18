namespace ends.outsider
{

    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BaRobotTower : BoardAgent
    {
        public void Setup(Board board, twin cell)
        {
            base.BoardSetup(board, cell);
        }

        private void FixedUpdate()
        {
            SetVelocityApproachTarget();
        }
    }

}