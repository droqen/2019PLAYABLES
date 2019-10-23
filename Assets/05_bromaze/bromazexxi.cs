using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;
using navdi3.xxi;

public class bromazexxi : BaseTilemapXXI
{
    public static bromazexxi Instance;

    public MazeMaster master;

    public EntityLot CreaturesLot {  get { return GetEntLot("creatures"); } }

    private void Start()
    {
        Instance = this;
        InitializeManualTT();
        banks["player"].Spawn<BroMazer>(GetEntLot("player")).Setup(master, twin.zero);

        new twinrect(-10, -9, 10, 9).DoEach(cell =>
        {
            if (// If in center 'circle'
                cell.sqrLength < Random.Range(30, 80)
            &&  // If at all gridaligned
                (Mathf.Abs(cell.x) % 2 == 0 || Mathf.Abs(cell.y) % 2 == 0)
            ) {
                // Open cell
                Sett(cell, Random.Range(0, 10));

                //if (cell.sqrLength > Random.Range(10, 160))
                //{
                //    // Spawn enemies more at edges of circle
                //    if (cell.x < 0)
                //    banks["pillbug"].Spawn<BroMazer>(CreaturesLot).Setup(master, cell);
                //    if (cell.x > 0)
                //    banks["pregghost"].Spawn<BroMazer>(CreaturesLot).Setup(master, cell);
                //}

            }
            else {
                Sett(cell, 10);
            }
        });

        new twinrect(-10, -9, 10, 9).DoEach(cell =>
        {
            if (Gett(cell) < 10
            && Gett(cell + twin.left) == 10
            && Gett(cell + twin.right) == 10) {

                bool downIsOpen = Gett(cell + twin.down) < 10;
                bool upIsOpen = Gett(cell + twin.up) < 10;

                if (downIsOpen && upIsOpen)
                {
                    // just a regular vertical corridor! ignore!
                } else if (downIsOpen)
                {
                    // spawner
                    banks["enter"].Spawn(GetEntLot("enters"), master.grid.GetCellCenterWorld(cell));
                } else if (upIsOpen)
                {
                    // exit
                    banks["exit"].Spawn(GetEntLot("exits"), master.grid.GetCellCenterWorld(cell));
                } else
                {
                    // sealed cavern
                    Sett(cell, 10);
                }
            }
        });
    }

    int queuedSpawns = 2;
    int toNextSpawn = 0;

    private void FixedUpdate()
    {
        toNextSpawn--;
        if (toNextSpawn < 0)
        {
            toNextSpawn = 120;
            queuedSpawns++;
        }

        if (queuedSpawns > 0)
        {
            var spawnTileCount = GetEntLot("enters").transform.childCount;
            if (spawnTileCount > 0)
            {
                var spawnTile = GetEntLot("enters").transform.GetChild(Random.Range(0, spawnTileCount));
                var cell = new twin(master.grid.WorldToCell(spawnTile.transform.position));

                if (Random.value < .5f)
                    banks["pillbug"].Spawn<BroMazer>(CreaturesLot).Setup(master, cell);
                else
                    banks["pregghost"].Spawn<BroMazer>(CreaturesLot).Setup(master, cell);

                queuedSpawns--;
            }
        }
    }



    public override int[] GetSolidTileIds()
    {
        return new int[] { 10 };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 50 };
    }

    public override void SpawnTileId(int TileId, Vector3Int TilePos)
    {
        throw new System.NotImplementedException();
    }
}
