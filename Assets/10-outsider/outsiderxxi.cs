namespace ends.outsider {

    using System.Collections;
    using System.Collections.Generic;
    using navdi3;
    using navdi3.maze;
    using navdi3.xxi;
    using UnityEngine;


    /*
     * 
     *  So the basic idea of outsider is that you're an outsider to a system: a troublemaker. the vagabond.
     *  Everyone in the world has got their things going on, and *IT'S COMING TO A CONCLUSION, LIKE IT OR NOT*!
     *  
     *  But you have a say in what that conclusion is.
     *
     *  the NEON ANTS are strictly organized and crawl along lines of light...
     *
     */


    public class outsiderxxi : BaseTilemapXXI
    {
        public static outsiderxxi Instance;

        MazeMaster master;

        public override int[] GetSolidTileIds()
        {
            return new int[] { 1, };
        }

        public override int[] GetSpawnTileIds()
        {
            return new int[] { 10, };
        }

        public override void SpawnTileId(int TileId, twin TilePos)
        {
            throw new System.NotImplementedException();
        }

        private void Start()
        {
            Instance = this;
            InitializeManualTT();
            new twinrect(0, 0, 19, 17).DoEach(cell => { Sett(cell, 0); });
            banks["player"].Spawn<PlayerMover>(GetEntLot("player")).Setup(mazeMaster, new twin(4,4));
        }

        public BombMaybeMover SpawnBomb(Vector3 position)
        {
            var bomb = banks["pbomb"].Spawn<BombMaybeMover>(GetEntLot("bombs"));
            bomb.Setup(mazeMaster, twin.zero);
            bomb.transform.position = position;
            return bomb;
        }

        public void SpawnExplosion(Vector3 pos)
        {
            banks["explosion"].Spawn<Explosion>(GetEntLot("explosions"), pos);
        }

    }

}