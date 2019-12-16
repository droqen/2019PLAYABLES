namespace ends.insider
{
    using navdi3.xxi;
    using navdi3.maze;
    using navdi3;

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class insiderxxi : BaseTilemapXXI
    {
        public twin[] oxygenIntakes;

        public twin bloodSpawnPoint;

        public float toNextBreath = 10;

        public AStarPather patherFindDanger;
        public AStarPather patherFindOxygen;
        public AStarPather patherFindRedblood;

        public override int[] GetSolidTileIds()
        {
            return new int[] { 1 };
        }

        public override int[] GetSpawnTileIds()
        {
            return new int[] { };
        }

        public override void SpawnTileId(int TileId, twin TilePos)
        {
            throw new System.NotImplementedException();
        }

        twinrect fullScreen = new twinrect(0, 0, 19, 17);
        twinrect playArea = new twinrect(1, 1, 18, 16);

        public static insiderxxi Instance;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            InitializeManualTT();
            for (int i = 0; i < 10; i++)
                if (GenerateNewLevel()) break;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                for (int i = 0; i < 10; i++)
                    if (GenerateNewLevel()) break;
            }

            toNextBreath -= Time.deltaTime;
            if (toNextBreath<0)
            {
                toNextBreath = 10;
                foreach(twin intake in oxygenIntakes)
                {
                    banks["oxygen"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, intake);
                    if (Random.value < .5f) banks["oxygen"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, intake);
                    if (Random.value < .25f) banks["oxygen"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, intake);
                }
            }
        }

        bool GenerateNewLevel()
        {
            // clear everything
            fullScreen.DoEach(cell => { Sett(cell, 1); });

            // generate list
            var cells = new List<twin>();
            playArea.DoEach(cell => { cells.Add(cell); });
            Util.shufl(ref cells);

            var open = new List<twin>();
            var closed = new List<twin>();

            open.Add(new twin(
                Random.Range(playArea.min.x, playArea.max.x + 1),
                Random.Range(playArea.min.y, playArea.max.y + 1)
            ));

            while(open.Count > 0)
            {
                var open_index = Random.Range(0, open.Count);
                var cell = open[open_index];
                open.RemoveAt(open_index);
                twin.ShuffleCompass();

                foreach (var dir in twin.compass)
                {
                    var ok = true;

                    foreach (var dx in new int[] { -1, 1 })
                    {
                        foreach (var dy in new int[] { -1, 1 })
                        {
                            if (Gett(cell + new twin(dx, dy)) == 0)
                            {
                                // empty corner. only allowed if it connects a proper path:
                                var path = 0;
                                if (Gett(cell + new twin(dx, 0)) == 0) path++;
                                if (Gett(cell + new twin(0, dy)) == 0) path++;
                                if (path != 1) ok = false;
                            }
                        }
                    }

                    if (ok && open.Count > 5 && Random.value < .25f) ok = false;

                    if (ok)
                    {
                        Sett(cell, 0);

                        var newopen = cell + dir;
                        if (!playArea.Contains(newopen)) continue;
                        if (open.Contains(newopen)) continue;
                        if (closed.Contains(newopen)) continue;
                        open.Add(newopen);

                        closed.Add(cell); // do i need this?
                    }
                    else
                    {
                        continue;
                    }
                }
            }


            int deadEndsCleanedUp = int.MaxValue;
            while(deadEndsCleanedUp>5)
            {
                deadEndsCleanedUp = 0;

                // clean up dead ends
                playArea.DoEach(cell =>
                {
                    if (Gett(cell) == 0)
                    {
                        var pathcount = 0;
                        foreach (var dir in twin.compass)
                        {
                            if (Gett(cell + dir) == 0) pathcount++;
                        }
                        if (pathcount < 2)
                        {
                            Sett(cell, 1);
                            deadEndsCleanedUp++;
                        }
                    }
                });
            }

            foreach (var agent in GetEntLot("guys").GetComponentsInChildren<InsiderBasicAgent>())
            {
                Object.Destroy(agent.gameObject);
            }

            var freeSpawnCells = new List<twin>();
            playArea.DoEach(cell => { if (Gett(cell) == 0) freeSpawnCells.Add(cell); });
            Util.shufl(ref freeSpawnCells);
            
            banks["player"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, freeSpawnCells[0]);

            bloodSpawnPoint = freeSpawnCells[1];

            for (int i = 0; i < 2; i++)
            {
                banks["redblood"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, bloodSpawnPoint);
                banks["whiteblood"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, bloodSpawnPoint);
            }

            oxygenIntakes = new twin[4];

            for (int i = 0; i < 4; i++)
            {
                oxygenIntakes[i] = freeSpawnCells[2 + i];
                banks["virus"].Spawn<InsiderBasicAgent>(GetEntLot("guys")).Setup(mazeMaster, oxygenIntakes[i]);
            }

            return true;
        }
    }

}