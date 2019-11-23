namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;


    using UnityEngine;
    using System.Collections;

    public class Agent_Pill : InsiderBasicAgent
    {
        public bool mode_bullet = true;
        public bool mode_medicine = false;
        public float medicine_energize = 0f;
        Vector2 bullet_velocity;

        public void Shoot(MazeMaster master, Vector3 position, twin shootdir)
        {
            base.Setup(master, twin.zero);
            transform.position = position + shootdir;
            body.velocity = bullet_velocity = 70 * (Vector2)shootdir;
            GetComponent<SpriteRenderer>().flipX = Random.value < .5f;
        }

        private void FixedUpdate()
        {
            if (mode_bullet)
            {
                GetComponent<BitsyAni>().speed = .4f;
                SnapMyCellPos();
                if ((body.velocity - bullet_velocity).sqrMagnitude > 10)
                {
                    mode_medicine = true;
                    mode_bullet = false;
                    var random_dirs = new ChoiceStack<twin>(twin.compass);
                    random_dirs.GetFirstTrue(TryMove);

                    body.velocity = Quaternion.Euler(0, 0, Random.Range(90, 270)) * bullet_velocity; // bounce backwards
                    GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX; // flip sprite
                }
            }
            else if (mode_medicine)
            {
                if (medicine_energize < 1)
                {
                    medicine_energize = Mathf.Clamp01(medicine_energize + .02f);
                    GetComponent<BitsyAni>().speed = Mathf.Lerp(.6f, 0, medicine_energize);
                    body.velocity *= 0.99f;
                }
                else
                {
                    GetComponent<BitsyAni>().speed = .12f;
                    if (IsWithinDistOfCentered(2f))
                    {
                        var dirs = new ChoiceStack<twin>(twin.compass);
                        dirs.RemoveAll(-lastMove);
                        lastMove = dirs.GetFirstTrue(TryMove);
                    }
                    else
                    {
                        body.velocity = body.velocity * .95f + .05f * (Vector2)ToCentered().normalized * 25f;
                    }
                }
            }
        }

    }

}