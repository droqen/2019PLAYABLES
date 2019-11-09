using System.Collections;
using System.Collections.Generic;
using navdi3;
using UnityEngine;

public class studyxxi : navdi3.xxi.BaseTilemapXXI
{
    public override int[] GetSolidTileIds()
    {
        return new int[] { 1, 2, };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 50 };
    }

    public override void SpawnTileId(int TileId, twin TilePos)
    {
        switch(TileId)
        {
            case 50: banks["player"].Spawn(GetEntLot("player"), tilemap.layoutGrid.GetCellCenterWorld(TilePos)); break;
        }
    }

    void Start()
    {
        InitializeTileSystem();
    }
}
