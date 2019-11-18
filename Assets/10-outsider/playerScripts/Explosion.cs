namespace ends.outsider
{
    using navdi3;
    using navdi3.maze;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Explosion : MonoBehaviour
    {
        // Update is called once per frame
        void FixedUpdate()
        {
            if (GetComponent<BitsyAni>().anim >= 3)
            {
                Object.Destroy(this.gameObject);
            }
        }
    }

}