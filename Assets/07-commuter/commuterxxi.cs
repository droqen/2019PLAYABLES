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
        return new int[] { 50, };
    }

    public override void SpawnTileId(int TileId, Vector3Int TilePos)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        InitializeManualTT();
        twinrect rect_outer = new twinrect(0, 0, 19, 17);
        twinrect rect_inner = new twinrect(rect_outer.min + twin.one, rect_outer.max - twin.one);
        twinrect rect_innerinner = new twinrect(rect_outer.min + twin.one * 2, rect_outer.max - twin.one * 2);

        rect_outer.DoEach(cell =>
        {
            if (rect_inner.Contains(cell) && (!rect_innerinner.Contains(cell) || Random.value < .7f))
            {
                if (Random.value < .05f && (cell-twin.one).taxicabLength > 3 && (cell-new twin(18,16)).taxicabLength > 3) Sett(cell, 1);
                else
                {
                    Sett(cell, 0);
                    if (Random.value < .16f)
                        banks["ant"].Spawn<CommMazer>(GetEntLot("ants")).Setup(master, cell);
                }
            }
            else
            {
                // border wall
                Sett(cell, 1);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AStarMapData ant_pathing;
    public twin? GetAntNextStep(CommMazer ant, twin target)
    {
        if (ant_pathing == null) ant_pathing = new AStarMapData(this, master, ant, new twinrect(0, 0, 19, 17));
        var path = MazeUtil.AStar(ant_pathing, ant.my_cell_pos, target);
        if (path == null)
        {
            return null;
        }
        else
        {
            foreach (var cell in path) if (cell != ant.my_cell_pos) return cell;
            // else...
            return target;
        }
    }
}
