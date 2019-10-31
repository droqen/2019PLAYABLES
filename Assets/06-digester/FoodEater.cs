using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEater : MonoBehaviour
{
    DigeMazer mazer {  get { return GetComponent<DigeMazer>(); } }
    navdi3.BitsyAni bani {  get { return GetComponent<navdi3.BitsyAni>(); } }

    public int amountToDigest = 0;
    public int starving = 0;

    private void FixedUpdate()
    {
        if (amountToDigest > 0)
        {
            amountToDigest--;
            if (amountToDigest <= 0)
            {
                mazer.speed = 8;
                bani.spriteIds = new int[] { 56, 57 };
            }
        }
        else
        {
            starving++;

            if (starving > 500)
            {
                starving++;
                mazer.speed = 20; // hurry!
                bani.spriteIds = new int[] { 66, 67 };
                bani.speed = 0.1f; // speed up the animation as well :(
            }

            if (starving > 1250)
            {
                Object.Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (amountToDigest <= 0)
        {
            switch(collision.gameObject.name) {
                case "food": Object.Destroy(collision.gameObject); amountToDigest = Random.Range(100,150); break;
                case "bigfood": Object.Destroy(collision.gameObject); amountToDigest = Random.Range(200,250); break;
            }

            if (amountToDigest > 0)
            {
                starving -= 500;
                if (starving < 0) starving = 0;
                mazer.speed = 0;
                bani.spriteIds = new int[] { 58, 59 };
                bani.speed = 0.05f;
            }
        }
    }
}
