namespace ends.outsider
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using navdi3;
    using navdi3.maze;

    public class Outsider : MazeBodyKinematic
    {
        Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
        public float moveSpeed = 30f;
        public override void FixedUpdate()
        {
            GetComponent<BitsyAni>().speed = 0.02f + 0.06f * body.velocity.magnitude / moveSpeed;

            base.FixedUpdate();
            var pin = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
                ).normalized;
            body.velocity = body.velocity * 0.6f + 0.4f * pin * moveSpeed;
        }
    }

}
