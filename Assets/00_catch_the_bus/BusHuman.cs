using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusHuman : MonoBehaviour
{
    SpriteRenderer spriter { get { return GetComponent<SpriteRenderer>(); } }

    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }

    BusDriver bus;

    float walkSpeed, enterSpeed, stayAwayFromClosedBus, rudeness;
    float avoidBumpingOthers;
    float rushOpenDoors;
    float hopefulDistance, hopefulRush, hopefulRudeness;

    bool is_hopeful;

    float enter_progress = 0.0f;
    float avoid_duration = 0.0f;
    Vector2 avoid_impulse;

    public void Setup(BusDriver bus)
    {
        this.bus = bus;
        spriter.color = new Color(
            Random.Range(0.5f, 1.0f),
            Random.Range(0.5f, 1.0f),
            Random.Range(0.5f, 1.0f));

        this.walkSpeed = Random.Range(5f,10f);
        this.enterSpeed = 2;
        this.stayAwayFromClosedBus = Random.Range(25f, 50f);
        this.rudeness = Random.Range(0f, 0.1f);

        this.avoidBumpingOthers = Random.value;
        this.rushOpenDoors = Random.Range(-4f, 8f);

        this.hopefulDistance = Random.Range(0f, 60f);
        this.hopefulRudeness = Random.value;
        this.hopefulRush = this.hopefulRudeness * Random.Range(5f, 20f);
    }

    private void Update()
    {
        if (inContactWithDoor && bus.IsEnterable)
        {
            enter_progress += enterSpeed;
            if (enter_progress > 1) Object.Destroy(this.gameObject);
        } else if (enter_progress > 0)
        {
            enter_progress -= enterSpeed;
        }

        var desiredSpeed = walkSpeed;

        if (avoid_duration > 0)
        {
            avoid_duration -= Time.deltaTime;

            body.velocity = avoid_impulse.normalized * desiredSpeed;
        }
        else
        {
            var toBus = (bus.doorTransform.position - this.transform.position);


            if (bus.IsEnterable)
            {
                if (toBus.magnitude < hopefulDistance) is_hopeful = true;
                desiredSpeed += rushOpenDoors;
                if (is_hopeful) desiredSpeed += this.hopefulRush;
            }
            if (!bus.IsEnterable)
            {
                if (is_hopeful) desiredSpeed += this.hopefulRush;
                else if (toBus.magnitude < stayAwayFromClosedBus) desiredSpeed = 0;
            }

            body.velocity = toBus.normalized * desiredSpeed;
        }
    }

    bool inContactWithDoor = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var currentRudeness = rudeness;
        if (is_hopeful) currentRudeness += hopefulRudeness;

        if (Random.value < currentRudeness)
        {
            return; // no reaction
        }

        if (collision.gameObject.GetComponent<BusHuman>()!=null)
        {
            var toOther = collision.transform.position - transform.position;

            avoid_impulse = Quaternion.Euler(0, 0, Random.value < 0.5f ? -90 : 90) * toOther.normalized;

            if (avoid_duration > 0 && Random.value < 0.5f) avoid_impulse = Vector2.zero; // try just stopping?

            avoid_duration = Random.value * avoidBumpingOthers;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var bus = collision.gameObject.GetComponent<BusDriver>();
        if (bus != null && Mathf.Abs(bus.body.velocity.x) > 10f) {
            if (Physics2D.Raycast((Vector2)transform.position + GetComponent<Collider2D>().offset, bus.body.velocity.x > 0 ? Vector2.left : Vector2.right).collider == collision.collider)
            {
                // TODO: spawn human corpse
                Object.Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform==bus.doorTransform)
        {
            inContactWithDoor = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform==bus.doorTransform)
        {
            inContactWithDoor = false;
        }
    }
}
