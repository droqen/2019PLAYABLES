using UnityEngine;
using System.Collections;

public class DirtBounce : MonoBehaviour
{
    public float gravity = .1f;
    float bottom_y;
    Vector2 velocity;
    int life;
    int freeze;
    private void Start()
    {
        bottom_y = transform.position.y - 3;
        //velocity = new Vector2(
        //    Random.Range(-.25f,.25f) + Random.Range(-.25f, .25f),
        //    Random.Range(.5f,.75f)
        //);
        life = Random.Range(25,75);
        freeze = 0;
        if (Random.value < .1f) freeze = 10;
    }
    private void FixedUpdate()
    {
        if (freeze > 0) freeze--;
        else
        {
            velocity.y -= gravity;
            transform.position += (Vector3)velocity;
            if (transform.position.y <= bottom_y)
            {
                velocity.x *= .5f;
                velocity.y *= -.5f;
                transform.position += Vector3.up * (bottom_y - transform.position.y);
            }

            life--;
            if (life < 25)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, life / 25f);
                if (life <= 0) Object.Destroy(this.gameObject);
            }
        }
    }
}
