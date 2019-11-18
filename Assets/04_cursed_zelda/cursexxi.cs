using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class cursexxi : navdi3.xxi.BaseTilemapXXI
{
    public navdi3.maze.MazeMaster master;

    private void Start()
    {
        InitializeTileSystem();
    }

    public override int[] GetSolidTileIds()
    {
        return new int[] { 1 };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 10, 11, 12, 13, };
    }

    public override void SpawnTileId(int TileId, navdi3.twin cell)
    {
        switch(TileId)
        {
            case 10: banks["player"].Spawn<CursedLinkPlayer>(GetEntLot("player")).Setup(master, cell); break;
            case 11: banks["wizard"].Spawn<navdi3.maze.MazeBody>(GetEntLot("wizard")).Setup(master, cell); break;
            case 12: banks["hole"].Spawn<HoleForBarrel>(GetEntLot("holes")).Setup(master, cell); break;
            case 13: banks["barrel"].Spawn<PushaBarrel>(GetEntLot("barrels")).Setup(master, cell); break;
        }
    }
}
