namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;


    using UnityEngine;
    using System.Collections;

    public enum AgentType
    {
        WhiteBlood=1, RedBlood=2, Pill=3, Player=4, Virus=5, Oxygen=6, BodyHeart=7,
    }

    public class InsiderBasicAgent : MazeBody
    {
        public AgentType agentType;

        public Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }

        public insiderxxi xxi { get { return insiderxxi.Instance; } }

        private void Start()
        {
            switch (agentType)
            {
                case AgentType.BodyHeart:
                    // spawn a bunch of blood cells nearby
                    xxi.banks["whiteblood"].Spawn<InsiderBasicAgent>(xxi.GetEntLot("guys")).Setup(xxi.mazeMaster, my_cell_pos);
                    xxi.banks["redblood"].Spawn<InsiderBasicAgent>(xxi.GetEntLot("guys")).Setup(xxi.mazeMaster, my_cell_pos);
                    xxi.banks["redblood"].Spawn<InsiderBasicAgent>(xxi.GetEntLot("guys")).Setup(xxi.mazeMaster, my_cell_pos);
                    xxi.banks["redblood"].Spawn<InsiderBasicAgent>(xxi.GetEntLot("guys")).Setup(xxi.mazeMaster, my_cell_pos);
                    break;
                default:
                    transform.position += Quaternion.Euler(0, 0, Random.value * 360) * twin.right * .5f;
                    break;
            }
        }

        private void FixedUpdate()
        {
            switch (agentType)
            {
                case AgentType.Player:
                    twin pin = new twin(Util.sign(Input.GetAxisRaw("Horizontal")), Util.sign(Input.GetAxisRaw("Vertical")));
                    body.velocity = body.velocity * 0.9f + 0.1f * (Vector2)pin * 50;

                    if (pin.taxicabLength == 1) GetComponent<FacingData>().facing = pin;

                    transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(twin.up, GetComponent<FacingData>().facing));
                    break;
                    
                case AgentType.Pill:
                    AgentScripts.PillFixedUpdate(this);
                    break;

                case AgentType.RedBlood:
                case AgentType.WhiteBlood:
                case AgentType.Virus:
                case AgentType.Oxygen:

                    if (IsWithinDistOfCentered(2f))
                    {
                        // great, keep going
                        var dirs = new ChoiceStack<twin>(twin.compass);
                        dirs.RemoveAll(-lastMove);
                        lastMove = dirs.GetFirstTrue(TryMove);
                    }
                    else
                    {
                        body.velocity = ToCentered().normalized * 20;
                    }

                    break;

                case AgentType.BodyHeart:
                    body.velocity = ToCentered().normalized * 20;
                    break;

            }
        }

        private void Update()
        {
            switch (agentType)
            {
                case AgentType.Player:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        AgentScripts.PillShootSetup(
                            master,
                            xxi.banks["pill"].Spawn<InsiderBasicAgent>(xxi.GetEntLot("guys")),
                            transform.position,
                            GetComponent<FacingData>().facing);
                    }
                    break;
            }
        }
    }

}