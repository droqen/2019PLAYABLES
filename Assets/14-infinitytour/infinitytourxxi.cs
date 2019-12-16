using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3.bitfont;
using navdi3.xxi;
using navdi3;

public class infinitytourxxi : BaseTilemapXXI
{
    public static infinitytourxxi Instance;

    public Camera gameCamera;

    public FontLot font;

    CharLot position_label;

    GameObject player;

    twin currentCamCell = new twin(-100, 100);

    private void Start()
    {
        Instance = this;
        InitializeManualTT();
        player = banks["player"].Spawn(GetEntLot("player"), mazeMaster.grid.GetCellCenterWorld(new twin(0,8)));

        position_label = CharLot.NewCharLot(font, "position", Vector3.zero, "0/0", gameCamera.transform);
        position_label.transform.localPosition = new Vector3(4-80, 4-72, 5);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = mazeMaster.grid.GetCellCenterWorld(new twin(0, 8));
        }

        twin camcell = new twin(Mathf.FloorToInt(player.transform.position.x / 160f), Mathf.FloorToInt(player.transform.position.y / 144f));
        gameCamera.transform.position = camcell.Scale(160, 144) + new Vector3(80, 72, -10);

        if (currentCamCell != camcell)
        {

            GetEntLot("losers").Clear();
            ClearAllTiles();

            Random.InitState(1024132 + camcell.x + 992 * camcell.y);

            float density = .2f + camcell.taxicabLength * .05f;

            var root = camcell.Scale(20, 18);
            var outer = new twinrect(root, root + new twin(20 - 1, 18 - 1));
            var inner = new twinrect(outer.min + twin.one, outer.max - twin.one);
            outer.DoEach(cell =>
            {
                if (cell.y == outer.mid.y || cell.y == outer.mid.y + 1 || cell.x == outer.mid.x || cell.x == outer.mid.x + 1)
                {
                    // if in center aisles and close to the edge
                    if (!inner.Contains(cell))
                    {
                        Sett(cell, 2); // 'no ai past this point'
                    }
                    else if (cell.x < inner.min.x + 3 || cell.x > inner.max.x - 3 || cell.y < inner.min.y + 2 || cell.y > inner.max.y - 2)
                    {
                        Sett(cell, 0);
                    }
                    // if in center aisles and close to the middle of the room
                    else
                    {
                        SpawnRandom(cell, density);
                    }
                }
                else if (inner.Contains(cell)) // if in meat of room and *not* in center aisles
                {
                    if (Random.value < density)
                        Sett(cell, 1);
                }
                else // if along outer edge
                {
                    Sett(cell, 1);
                }

            });


            if (camcell.y > currentCamCell.y)
            {
                var jumper = player.GetComponent<navdi3.jump.Jumper>();
                jumper.body.velocity = new Vector2(jumper.body.velocity.x, jumper.y_JumpSpeed);
            }

            currentCamCell = camcell;

            var xlabel = "--";
            if (camcell.x != 0) xlabel = Mathf.Abs(camcell.x) + (camcell.x < 0 ? "w" : "e");
            var ylabel = "--";
            if (camcell.y != 0) ylabel = Mathf.Abs(camcell.y) + (camcell.y < 0 ? "s" : "n");

            position_label.Print(xlabel+"/"+ylabel);
        }
    }

    void SpawnRandom(twin cell, float density)
    {
        var r = Random.Range(0, 1 + density);
        if (r > 1.2f)
        {
            var loser = banks["loser"].Spawn<MazeLoser>(GetEntLot("losers"));
            loser.Setup(mazeMaster, cell);

            Dj.Tempf("spawned loser {0} parent {1}", loser, loser.transform.parent);
        }
        else if (r > 1)
        {
            Sett(cell, 1);
        }
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
