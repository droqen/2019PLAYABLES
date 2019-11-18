namespace ends.outsider
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    using navdi3;
    using navdi3.xxi;
    using navdi3.maze;

    public class outerboardxxi : BaseTilemapXXI
    {
        public List<twin> openCells;
        public List<twin> openSpawnCells;

        Board board;

        private void Start()
        {
            InitializeManualTT();

            this.board = new Board(mazeMaster, this);
            Util.untiltrue(TryGenerateValidBoard);
        }

        private void Update()
        {
            //if (Input.GetKey(KeyCode.Space)) this.board.NextGenStep();
            if (Input.GetKeyDown(KeyCode.R))
            {
                Util.untiltrue(TryGenerateValidBoard);
            }
        }

        public bool TryGenerateValidBoard()
        {
            this.board.ResetGen();
            for (int i = 0; i < 2000; i++) if (!this.board.NextGenStep()) break;

            // check validity, rejection

            this.openCells = new List<twin>();
            new twinrect(0, 0, 19, 17).DoEach(cell => { if (Gett(cell) == 0) openCells.Add(cell); });
            if (openCells.Count < 80) return false; // not enough open cells.

            Util.shufl(ref openCells);

            this.openSpawnCells = new List<twin>(openCells);
            for (int i = 0; i < openSpawnCells.Count; i++)
                for (int j = i + 1; j < openSpawnCells.Count; j++)
                    if ((openSpawnCells[i] - openSpawnCells[j]).taxicabLength <= 2)
                        { openSpawnCells.RemoveAt(j); j--; }

            if (openSpawnCells.Count < 20) return false; // crash if not enough spawn cells

            foreach (var child in GetEntLot("agents").transform) Object.Destroy(((Transform)child).gameObject);

            banks["neon nest"].Spawn<BaNeonNest>(GetEntLot("ants")).Setup(board, openSpawnCells[0]);

            banks["food source"].Spawn<BaFoodSource>(GetEntLot("foods")).Setup(board, openSpawnCells[1]);
            banks["food source"].Spawn<BaFoodSource>(GetEntLot("foods")).Setup(board, openSpawnCells[2]);

            banks["blue flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[3]);
            banks["blue flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[4]);

            banks["red flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[5]);
            banks["red flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[6]);

            banks["evil flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[7]);
            banks["evil flower"].Spawn<BaPlant>(GetEntLot("plants")).Setup(board, openSpawnCells[8]);

            banks["food source"].Spawn<BaFoodSource>(GetEntLot("foods")).Setup(board, openSpawnCells[9]);

            // spawn just two robots.
            banks["robot boy"].Spawn<BaRobotBoy>(GetEntLot("robots")).Setup(board, openSpawnCells[10]);
            banks["robot boy"].Spawn<BaRobotBoy>(GetEntLot("robots")).Setup(board, openSpawnCells[10]);


            // spawn a player.
            banks["player"].Spawn<Outsider>(GetEntLot("player")).Setup(mazeMaster, openSpawnCells[11]);

            //for (int i = 0; i < 3; i++)
            //    banks["wander_agent"].Spawn<BATeamCopier>(GetEntLot("agents")).TeamCopierSetup(this.board, openCells[0], 0);

            //for (int i = 0; i < 3; i++)
            //    banks["wander_agent"].Spawn<BATeamCopier>(GetEntLot("agents")).TeamCopierSetup(this.board, openCells[1], 1);

            return true;

        }




        public override int[] GetSolidTileIds()
        {
            return new int[] { 1 };
        }

        public override int[] GetSpawnTileIds()
        {
            return new int[] { 50 };
        }

        public override void SpawnTileId(int TileId, twin TilePos)
        {
            throw new System.NotImplementedException();
        }
    }

}