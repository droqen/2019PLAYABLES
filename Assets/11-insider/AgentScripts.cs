namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;

    using UnityEngine;

    public static class AgentScripts
    {

        public static void PillShootSetup(MazeMaster master, InsiderBasicAgent agent, Vector3 position, twin shootdir)
        {
            agent.Setup(master, twin.zero);
            agent.transform.position = position + shootdir;
            agent.body.velocity = agent.GetComponent<PillData>().bullet_velocity = 70 * (Vector2)shootdir;
            agent.GetComponent<SpriteRenderer>().flipX = Random.value < .5f;
        }

        public static void PillFixedUpdate(InsiderBasicAgent agent)
        {
            var pd = agent.GetComponent<PillData>();
            if (pd.mode_bullet)
            {
                agent.GetComponent<BitsyAni>().speed = .4f;
                agent.SnapMyCellPos();
                if ((agent.body.velocity - pd.bullet_velocity).SqrMagnitude() > 10)
                {
                    pd.mode_medicine = true;
                    pd.mode_bullet = false;
                    var random_dirs = new ChoiceStack<twin>(twin.compass);
                    random_dirs.GetFirstTrue(agent.TryMove);

                    agent.body.velocity = Quaternion.Euler(0, 0, Random.Range(90, 270)) * pd.bullet_velocity; // bounce backwards
                    agent.GetComponent<SpriteRenderer>().flipX = !agent.GetComponent<SpriteRenderer>().flipX; // flip sprite
                }
            }
            else if (pd.mode_medicine)
            {
                if (pd.medicine_energize < 1)
                {
                    pd.medicine_energize = Mathf.Clamp01(pd.medicine_energize + .02f);
                    agent.GetComponent<BitsyAni>().speed = Mathf.Lerp(.6f, 0, pd.medicine_energize);
                    agent.body.velocity *= 0.99f;
                }
                else
                {
                    agent.GetComponent<BitsyAni>().speed = .12f;
                    if (agent.IsWithinDistOfCentered(2f))
                    {
                        var dirs = new ChoiceStack<twin>(twin.compass);
                        dirs.RemoveAll(-agent.lastMove);
                        agent.lastMove = dirs.GetFirstTrue(agent.TryMove);
                    }
                    else
                    {
                        agent.body.velocity = agent.body.velocity * .95f + .05f * (Vector2)agent.ToCentered().normalized * 25f;
                    }
                }
            }
        }
    }

}