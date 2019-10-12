using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

[RequireComponent(typeof(BankLot))]
[RequireComponent(typeof(SpriteLot))]

public class BusXXI : MonoBehaviour
{
    public Transform humanSpawnPoint;
    public Transform humanSpawnLine;

    public BusDriver bus;

    int spawns_left = 10;

    float next_group_spawn = 0.0f;

    public BankLot banks { get { return GetComponent<BankLot>(); } }
    public SpriteLot sprites { get { return GetComponent<SpriteLot>(); } }

    protected Dictionary<string, EntityLot> entlots = new Dictionary<string, EntityLot>();
    public EntityLot GetEntLot(string entLotName)
    {
        if (!entlots.ContainsKey(entLotName)) entlots.Add(entLotName, EntityLot.NewEntLot(entLotName));
        return entlots[entLotName];
    }

    private void Start()
    {
        for (int i = 0; i < spawns_left; i++) SpawnHuman(humanSpawnLine, Vector2.right * Random.Range(-10f,10f));
        spawns_left = 0;
        next_group_spawn = Time.time + Random.Range(11f, 14f) * 0.5f;
    }

    private void Update()
    {
        if (Time.time > next_group_spawn)
        {
            spawns_left += Random.Range(1, 10);
            next_group_spawn = Time.time + Random.Range(11f, 14f);
        }

        if (spawns_left > 0)
        {
            SpawnHuman(humanSpawnPoint);
            spawns_left--;
        }
    }

    void SpawnHuman(Transform where, Vector2 offset = default(Vector2))
    {
        banks["human"].Spawn<BusHuman>(GetEntLot("humans"), (Vector2)where.transform.position + offset).Setup(bus);
    }
}
