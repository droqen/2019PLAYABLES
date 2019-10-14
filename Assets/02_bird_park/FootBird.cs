using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootBird : MonoBehaviour
{
    List<Repeller> repels = new List<Repeller>();

    public Sprite[] bird_sprites;
    public float speed_mult = 1.0f;
    public float speed_max = 5.0f;
    float anim;
    float anim_rate;
    Vector3 target;
    private void Start()
    {
        Retarget();
        this.transform.position = target;
        anim = Random.value * 2;
        anim_rate = Random.Range(2f, 2.2f);
    }
    private void Update()
    {
        anim = (anim + anim_rate * Time.deltaTime) % 2;
        GetComponent<SpriteRenderer>().sprite = bird_sprites[(int)anim];
        GetComponent<SpriteRenderer>().flipX = target.x < transform.position.x;
        
        if (Random.value < 0.01f) Retarget();
        var v = (target - transform.position) * speed_mult;
        if (v.sqrMagnitude > speed_max * speed_max)
            v = v.normalized * speed_max;

        foreach (var r in repels) v = r.ApplyToVelocity(v, Time.deltaTime);
        repels.RemoveAll(r => { return r.strength <= 0; });

        GetComponent<Rigidbody2D>().velocity = v;
    }
    private void Retarget()
    {
        target = new navdi3.twin(Random.Range(0,160),Random.Range(0,144));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        repels.Add(new Repeller(transform, target, collision.transform, 1.0f));
    }
}

class Repeller
{
    Transform myTransform;
    Transform repellantTransform;
    public float strength;
    public Repeller(Transform self, Vector3? my_target, Transform repellant, float strength = 1.0f)
    {
        myTransform = self;
        repellantTransform = repellant;
    }
    public Vector2 ApplyToVelocity(Vector2 v, float deltatime)
    {
        var dir = myTransform.position - repellantTransform.position;
        if (dir.sqrMagnitude < 20 * 20) strength += deltatime * 3;
        else strength -= deltatime;
        v += (Vector2)dir.normalized * strength * deltatime;
        return v;
    }
}
