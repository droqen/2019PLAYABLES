using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class cursexxi : navdi3.xxi.BaseTilemapXXI
{
    public navdi3.maze.MazeMaster master;

    public TextAsset sokoTextAsset;

    List<Sokolevel> levels;
    List<twin> locs;

    void SpawnSokolevelAt(twin pos, Sokolevel level)
    {
        level.DoEach(pos, (cell, tileType) =>
        {
            switch (tileType)
            {
                case Sokolevel.Tile.Boulder: banks["barrel"].Spawn<PushaBarrel>(GetEntLot("barrel")).Setup(master, cell); break;
                case Sokolevel.Tile.Floor: break;
                case Sokolevel.Tile.Goal: banks["goal"].Spawn<TargetForBarrel>(GetEntLot("goal")).Setup(master, cell); break;
                case Sokolevel.Tile.Wall: Sett(cell, 1); break;
                case Sokolevel.Tile.Player: 
                    banks["player"].Spawn<CursedLinkPlayer>(GetEntLot("player")).Setup(master, cell);
                    break;
            }
        });
    }

    private void Start()
    {
        levels = Sokoparse.ParseFullAsset(sokoTextAsset);
        locs = Sokopack.GetPackedLevelLocations(levels);

        InitializeManualTT();

        SpawnSokolevelAt(twin.zero, levels[0]);

        //var outer = new twinrect(0, 0, 19, 18);
        //var inner = new twinrect(outer.min + twin.one, outer.max - twin.one);
        //outer.DoEach(cell =>
        //{
        //    if (!inner.Contains(cell)) Sett(cell, 1);
        //    else if (Random.value < .25f)
        //    {
        //        banks["barrel"].Spawn<PushaBarrel>(GetEntLot("barrel")).Setup(master, cell);
        //    }
        //    else if (Random.value < .125f)
        //    {
        //        banks["hole"].Spawn<HoleForBarrel>(GetEntLot("hole")).Setup(master, cell);
        //    }
        //    else if (Random.value < .125f)
        //    {
        //        banks["goal"].Spawn<TargetForBarrel>(GetEntLot("goal")).Setup(master, cell);
        //    }
        //});

        //banks["player"].Spawn<CursedLinkPlayer>(GetEntLot("player")).Setup(master, new twin(9, 9));
        //banks["player"].Spawn<CursedLinkPlayer>(GetEntLot("player")).Setup(master, new twin(3,3));
    }

    public override int[] GetSolidTileIds()
    {
        return new int[] { 1 };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 50 };
    }

    public override void SpawnTileId(int TileId, navdi3.twin TilePos)
    {
        throw new System.NotImplementedException();
    }
}
