namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;


    using UnityEngine;
    using System.Collections;

    public class Agent_Player : InsiderBasicAgent
    {
        twin facing = twin.up;

        private void FixedUpdate()
        {
            twin pin = new twin(Util.sign(Input.GetAxisRaw("Horizontal")), Util.sign(Input.GetAxisRaw("Vertical")));
            body.velocity = body.velocity * 0.9f + 0.1f * (Vector2)pin * 50;

            if (pin.taxicabLength == 1) facing = pin;

            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(twin.up, facing));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                xxi.banks["pill"].Spawn<Agent_Pill>(xxi.GetEntLot("guys")).Shoot(master, transform.position, facing);
            }
        }
    }

}