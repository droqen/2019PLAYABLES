using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneStarXXI : navdi3.xxi.BaseTilemapXXI
{

    private void Start()
    {
        InitializeManualTT();
    }

    public override int[] GetSolidTileIds()
    {
        return new int[] { 1 };
    }

    public override int[] GetSpawnTileIds()
    {
        throw new System.NotImplementedException();
    }

    public override void SpawnTileId(int TileId, navdi3.twin TilePos)
    {
        throw new System.NotImplementedException();
    }
}
