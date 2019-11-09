using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.maze;

public class commuterxxi : navdi3.xxi.BaseTilemapXXI
{
    public static commuterxxi Instance;
    public MazeMaster master { get { return tilemap.GetComponentInParent<MazeMaster>(); } }

    public override int[] GetSolidTileIds()
    {
        return new int[] { 1, };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int[] { 50, 53, };
    }

    public override void SpawnTileId(int TileId, twin cell)
    {
        Vector3 EntPos = master.grid.GetCellCenterWorld(cell);
        switch(TileId)
        {
            case 50: banks["ant"].Spawn<CommuterAnt>(GetEntLot("ants")).Setup(master, cell, pather_solids); break;
            case 53: banks["player"].Spawn<CommuterPlayer>(GetEntLot("player")).Setup(master, cell, pather_solids); break;
            default: throw new System.NotImplementedException();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        InitializeAStar();
        InitializeTileSystem();
        //InitializeManualTT();
        twinrect rect_outer = new twinrect(0, 0, 19, 17);
        twinrect rect_inner = new twinrect(rect_outer.min + twin.one, rect_outer.max - twin.one);
        twinrect rect_innerinner = new twinrect(rect_outer.min + twin.one * 2, rect_outer.max - twin.one * 2);

        rect_outer.DoEach(cell =>
        {
            if ( rect_inner.Contains(cell) )
            {
                float chance_of_wall = .02f;
                if (cell.x % 2 == 0) chance_of_wall += .1f;
                if (cell.y % 2 == 0) chance_of_wall += .1f;

                if (Random.value < chance_of_wall && (cell - twin.one).taxicabLength > 3 && (cell - new twin(18, 16)).taxicabLength > 3)
                {
                    Sett(cell, 1);
                }
                else
                {
                    Sett(cell, 0);
                    if (Random.value < .12f)
                        banks["ant"].Spawn<CommuterAnt>(GetEntLot("ants")).Setup(master, cell, pather_solids);
                }
            }
            else
            {
                // border wall
                Sett(cell, 1);
            }
        });

        banks["player"].Spawn<BaseCommuter>(GetEntLot("player")).Setup(master, new twin(18,16), pather_solids);
    }

    int to_next_destink = 0;

    void FixedUpdate()
    {
        //if (to_next_destink > 0) to_next_destink--;
        //else
        //{
        //    Dictionary<twin, float> newstink = new Dictionary<twin, float>();
        //    foreach (var cell in stinkycells.Keys)
        //    {
        //        newstink[cell] = stinkycells[cell] * 0.5f;
        //    }
        //    stinkycells = newstink;
        //    to_next_destink = 60;
        //}
    }

    public AStarPather pather_solids;
    Dictionary<twin, float> stinkycells;
    void InitializeAStar()
    {
        pather_solids = new AStarPather(new MazeWalkerLambda((a, b) => {
            return SolidTileIds.Contains(Gett(b)) ? int.MaxValue : 1 + GetStink(b);
        }), new twinrect(0, 0, 19, 17));

        stinkycells = new Dictionary<twin, float>();
    }
    public void AddStink(twin cell, float stinkiness)
    {
        if (!stinkycells.ContainsKey(cell)) stinkycells[cell] = stinkiness;
        else stinkycells[cell] += stinkiness;
    }
    int GetStink(twin cell)
    {
        if (stinkycells.TryGetValue(cell, out var stink))
        {
            return (int)stink;
        }
        else
        {
            return 0;
        }
    }
}
