namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;


    using UnityEngine;
    using System.Collections;

    public class Agent_Whitecell : InsiderBasicAgent
    {
        private void FixedUpdate()
        {
            if (IsWithinDistOfCentered(2f))
            {
                // great, keep going
                var dirs = new ChoiceStack<twin>(twin.compass);
                dirs.RemoveAll(-lastMove);
                lastMove = dirs.GetFirstTrue(TryMove);
            } else
            {
                body.velocity = ToCentered().normalized * 20;
            }
        }
    }

}