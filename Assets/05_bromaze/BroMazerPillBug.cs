using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class BroMazerPillBug : BroMazer
{
    public bool balled;
    public int balledDuration = 0;

    public PhysicsMaterial2D balledMaterial;
    public PhysicsMaterial2D unballedMaterial;

    private void FixedUpdate()
    {
        SpriterUpdate();

        if (balled)
        {

            if (this.balledDuration > 0)
            {
                if (body.velocity.sqrMagnitude < .01f) this.balledDuration--;
            }
            else SetBalled(false);

        } else {

            MoveUpdate_Center(baseSpeed);
            if (CheckIsFrustrated(40))
            {
                SetBalled(true);
            }

            if (IsWithinDistOfCentered(2f))
            {
                ChooseNewDir_Forward();
            }

        }
    }

    void SetBalled(bool value)
    {
        if (this.balled != value) {

            this.balled = value;

            if (value) {
                this.balledDuration = Random.Range(60, 300);
                //body.sharedMaterial = balledMaterial;
            } else
            {
                //body.sharedMaterial = unballedMaterial;
            }

            RefreshCell();
            lastMove = twin.zero;

            for (var i = 0; i < bani.spriteIds.Length; i++) bani.spriteIds[i] = 54 + i + (value ? 10 : 0);
        }
    }
}
