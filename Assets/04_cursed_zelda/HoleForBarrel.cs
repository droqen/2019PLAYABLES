using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class HoleForBarrel : navdi3.maze.MazeBody
{
    public cursexxi xxi;
    // trigger
    public bool full = false;
    public void FillMeUp()
    {
        //new twinrect(0, 0, 2, 2).DoEach(twinoffset =>
        //{
        //    if (twinoffset.x == 0 || twinoffset.x == 2)
        //        if (twinoffset.y == 0 || twinoffset.y == 2)
        //            return;
        //    xxi.banks["darkparticle"].Spawn<DarkParticle>(xxi.GetEntLot("particles")).Setup(this, twinoffset * 2 + new twin(-2,-2));
        //});
        full = true;
        GetComponent<navdi3.BitsyAni>().speed = Random.Range(0.01f, 0.02f);
        GetComponent<navdi3.BitsyAni>().spriteIds = new int[] { 70, 71 };
    }
    private void FixedUpdate()
    {
        if (!full)
        {
            GameObject fillmewith = null;
            foreach (var mazebody in master.GetBodiesAt(my_cell_pos))
            {
                if (mazebody != this) {
                    Vector2 pullVector = this.transform.position - mazebody.transform.position;
                    if (pullVector.sqrMagnitude < 1) fillmewith = mazebody.gameObject;
                    mazebody.GetComponent<Rigidbody2D>().velocity += pullVector.normalized * 1.0f; // pull them in
                }
            }

            if (fillmewith)
            {
                Object.Destroy(fillmewith);
                FillMeUp();
            }
        }
    }
}
