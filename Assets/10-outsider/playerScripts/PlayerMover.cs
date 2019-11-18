namespace ends.outsider
{
    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerMover : MazeBody
    {
        Vector3 lastNonzeroPin = twin.right;

        public float playerMoveSpeed = 40f;

        void FixedUpdate()
        {
            var pin = new Vector2(
                x: Input.GetAxisRaw("Horizontal"),
                y: Input.GetAxisRaw("Vertical"));

            if (pin.sqrMagnitude > .1f) lastNonzeroPin = pin;

            GetComponent<Rigidbody2D>().velocity = pin.normalized * playerMoveSpeed;

            SnapMyCellPos();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var shootdir = (lastNonzeroPin + Quaternion.Euler(0, 0, Random.value * 360) * new Vector3(.25f, 0, 0)).normalized;
                outsiderxxi.Instance.SpawnBomb(transform.position + lastNonzeroPin * .1f).GetComponent<Rigidbody2D>().velocity = shootdir * playerMoveSpeed * 2;
            }
        }
    }

}