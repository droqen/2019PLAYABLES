namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;


    using UnityEngine;
    using System.Collections;

    abstract public class InsiderBasicAgent : MazeBody
    {
        public Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }

        public insiderxxi xxi { get { return insiderxxi.Instance; } }

    }

}