namespace ends.outsider
{

    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BombMaybeMover : MazeBody
    {
        public int lifespan = 200;
        private void FixedUpdate()
        {
            lifespan--;
            if (lifespan > 0)
            {
                GetComponent<BitsyAni>().speed = Util.remap(0, 200, .25f, .01f, this.lifespan);
            }
            else
            {
                outsiderxxi.Instance.SpawnExplosion(this.transform.position);
                Object.Destroy(this.gameObject);
            }

            GetComponent<Rigidbody2D>().velocity *= 0.96f;
        }
    }

}