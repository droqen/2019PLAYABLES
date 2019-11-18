namespace ends.outsider
{

    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BoardAgent : MazeBody
    {

        public float accelRate = .25f;
        public float moveSpeed = 20f;
        public float fullSpeedDist = 8f;
        public float maxOffset = 3f;
        public Vector2 lastFrameVelocity;
        public int bodyStuckFrames;

        override public void OnMoved(twin prev_pos, twin target_pos) {
            this.board.UpdateAgentCellPos(this, target_pos);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            bumpedThisFrame++;
        }

        protected int bumpedThisFrame = 0;
        protected Vector2 subtileOffset = Vector2.zero;
        
        public Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }

        protected Board board { get; private set; }
        protected outerboardxxi xxi { get; private set; }

        public void BoardSetup(Board board, twin cell)
        {
            this.board = board;
            this.board.AddAgent(this); // do better tracking
            this.xxi = board.xxi;
            base.Setup(board.master, cell);

            RandomizeSubtileOffset();
        }

        public void RandomizeSubtileOffset()
        {
            this.subtileOffset = new Vector2(Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset));
        }

        public bool WasVelocityAltered(float alteredAmount = float.Epsilon)
        {
            return (body.velocity - lastFrameVelocity).sqrMagnitude > alteredAmount;
        }

        public void SetVelocityApproachTarget()
        {
            if (WasVelocityAltered()) bodyStuckFrames++;
            else bodyStuckFrames = 0;

            var toTarget = (Vector2)ToCentered() + subtileOffset;
            var targetVelocity = Vector2.zero;
            if (toTarget.sqrMagnitude > fullSpeedDist * fullSpeedDist)
                targetVelocity = toTarget.normalized * moveSpeed;
            else
                targetVelocity = toTarget / fullSpeedDist * moveSpeed;

            body.velocity = body.velocity * (1-accelRate) + (accelRate) * targetVelocity;
            lastFrameVelocity = body.velocity;
        }

        private void OnDestroy()
        {
            this.board.RemoveAgent(this);
        }
    }

}
