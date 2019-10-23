using UnityEngine;
using System.Collections;

public class BroMazerPlayer : BroMazer
{
    private void FixedUpdate()
    {
        var pinMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        pinMove.Normalize();

        SpriterUpdate();
        MoveUpdate_TargetVelocity(pinMove * baseSpeed);
    }
}
