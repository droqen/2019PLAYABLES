using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.xxi;
using navdi3.maze;

public class digesterxxi : BaseTilemapXXI
{
    public static digesterxxi Instance;
    public MazeMaster master {  get { return tilemap.GetComponentInParent<MazeMaster>(); } }

    int toNextFoodSpawn = 10;

    private void Awake()
    {
        Instance = this;
    }

    ChoiceStack<twin> foodSpawnPoints = new ChoiceStack<twin>();

    private void Start()
    {
        InitializeTileSystem();

        foodSpawnPoints.Lock();

        //InitializeManualTT();
        //var outer = new twinrect(0, 0, 20-1, 18-1);
        //outer.DoEach(cell => { Sett(cell, 1); });
        //var inner = outer;
        //inner.min += twin.one;
        //inner.max -= twin.one;
        //inner.DoEach(cell => {

        //    Sett(cell, 0);

        //    if (Random.value < .1f)
        //    {
        //        banks["food"].Spawn<MazeBody>(GetEntLot("food")).Setup(master, cell);
        //    }
        //    else if (Random.value < .05f)
        //    {
        //        banks["bigfood"].Spawn<MazeBody>(GetEntLot("food")).Setup(master, cell);
        //    }
        //    else if (Random.value < .04f)
        //    {
        //        banks["eater"].Spawn<MazeBody>(GetEntLot("guts")).Setup(master, cell);
        //    }

        //});

        //banks["player"].Spawn<MazeBody>(GetEntLot("player")).Setup(master, new twin(10, 8));
    }

    private void FixedUpdate()
    {
        toNextFoodSpawn--;
        if (toNextFoodSpawn <= 0)
        {
            toNextFoodSpawn = 60;
            var spawnName = Random.value < .75f ? "food" : "bigfood" ;
            banks[spawnName].Spawn<MazeBody>(GetEntLot("food")).Setup(master, foodSpawnPoints.GetRandom());
        }
    }

    public override int[] GetSolidTileIds()
    {
        return new int[] { 1 };
    }

    public override int[] GetSpawnTileIds()
    {
        return new int [] { 50, 52, 56, };
    }

    public override void SpawnTileId(int TileId, navdi3.twin cell)
    {
        switch(TileId)
        {
            case 50:
                banks["player"].Spawn<MazeBody>(GetEntLot("player")).Setup(master, cell);
                //Sett(cell, 0);
                break;
            case 52:
                foodSpawnPoints.Add(cell);
                //Sett(cell, 0);
                break;
            case 56:
                banks["eater"].Spawn<MazeBody>(GetEntLot("guts")).Setup(master, cell);
                //Sett(cell, 0);
                break;
        }
    }
}
