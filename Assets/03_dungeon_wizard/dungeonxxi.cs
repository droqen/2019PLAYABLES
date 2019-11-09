using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class dungeonxxi : navdi3.xxi.BaseTilemapXXI
{
    public static dungeonxxi Instance { get; private set; }

    public navdi3.maze.MazeMaster mazeMaster;

    PlayerBodyCtrl player;
    GameObject cursor;

    private void Start()
    {
        GetSolidTileIds();

        Instance = this;
        InitializeManualTT();
        new twinrect(twin.zero, new twin(32, 32) - twin.one).DoEach(cell =>
            {
                Sett(cell, 2); // initialize whole screen to solid
            });

        twin center = new twin(Random.Range(5, 32 - 5), Random.Range(5, 32 - 5));
        new twinrect(center - twin.one, center + twin.one).DoEach(cell =>
            {
                Dig(cell, dirty:false);
            });

        Sett(center, 1);

        player = banks["tinyplayer"].Spawn<PlayerBodyCtrl>(GetEntLot("player"), tilemap.GetCellCenterWorld(center + twin.down));
        cursor = banks["tinycursor"].Spawn(GetEntLot("cursor"));
        cursor.gameObject.SetActive(false);

        banks["badghost"].Spawn<BadGhost>(GetEntLot("ghosts")).Setup(mazeMaster, center + twin.up);
        banks["badghost"].Spawn<BadGhost>(GetEntLot("ghosts")).Setup(mazeMaster, center + twin.up);
        banks["pigeon"].Spawn<BadPigeon>(GetEntLot("ghosts")).Setup(mazeMaster, center + twin.up);
        banks["pigeon"].Spawn<BadPigeon>(GetEntLot("ghosts")).Setup(mazeMaster, center + twin.up);

    }

    void Dig(twin cell, bool dirty = true) // dirty is the default!
    {
        twinrect dirtyrect = new twinrect(0, 0, 3, 3);
        if (Gett(cell) == 3) dirtyrect.min += twin.up * 2;
        Sett(cell, 0);
        if (Gett(cell + twin.up) == 2)
        {
            Sett(cell + twin.up, 3);
            dirtyrect.max += twin.up * 2;
        }

        SpawnDirtParts(cell, dirtyrect);
    }

    void SpawnDirtParts(twin cell, twinrect subpartrect)
    {
        var bottomleftpos = tilemap.CellToWorld(cell) + twin.one;
        subpartrect.DoEach(subpart =>
        {
            banks["dirtpart"].Spawn(GetEntLot("dirts"), bottomleftpos + subpart * 2);
        });
    }

    private void Update()
    {
        if (player.facing != twin.zero)
        {
            // project
            twin playerscell = new twin(tilemap.layoutGrid.WorldToCell(player.transform.position));
            twin targetcell = playerscell + player.facing;
            if (IsSolidTile(targetcell))
            {
                cursor.transform.position = tilemap.layoutGrid.GetCellCenterWorld(targetcell);
                cursor.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Dig(targetcell);
                }
            } else
            {
                cursor.gameObject.SetActive(false);
            }
        }
    }

    public twin? GetRandomFreeCell(System.Func<twin,bool> cellVerifier)
    {
        var freeCells = new ChoiceStack<twin>();
        new twinrect(twin.zero, new twin(32, 32) - twin.one).DoEach(cell =>
        {
            if (!IsSolidTile(cell) && cellVerifier(cell)) freeCells.Add(cell);
        });
        freeCells.Lock(shuffle:true);
        var freeCell = freeCells.GetFirstTrue(cell => { return true; }, defaultValue: twin.one*-1000);
        if (freeCell == twin.one * -1000) return null; else return freeCell;
    }



    bool IsSolidTile(twin cell)
    {
        return SolidTileIds.Contains(Gett(cell));
    }



    int[] SolidTileIdsArray;
    HashSet<int> SolidTileIds;

    public override int[] GetSolidTileIds()
    {
        if (SolidTileIds == null) SolidTileIds = new HashSet<int>(SolidTileIdsArray = new int[]{ 2,3, 4,5,6, });
        return SolidTileIdsArray;
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 1 };
        //throw new System.NotImplementedException();
    }

    public override void SpawnTileId(int TileId, navdi3.twin TilePos)
    {
        throw new System.NotImplementedException();
    }
}
