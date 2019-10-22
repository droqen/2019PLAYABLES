using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class DarkParticle : MonoBehaviour
{
    Vector2 velocity;
    public void Setup(HoleForBarrel hole, twin offset)
    {
        transform.position = hole.transform.position + offset;
        GetComponent<BitsyAni>().speed *= Random.Range(.8f, 1.2f);
    }
    private void FixedUpdate()
    {
        //transform.position += Vector3.down * .2f;
        if (GetComponent<BitsyAni>().anim > 1.75f) Object.Destroy(this.gameObject);

    }
}
