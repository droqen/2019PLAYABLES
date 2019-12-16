using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;
using navdi3.xxi;


public class bonsaixxi : BaseTilemapXXI
{
    public static bonsaixxi Instance;

    [Header("bonsaixxi values")]
    public Camera gameCamera;
    public float currentOrthographicSize = 10;
    public float currentOrthographicVelocity = 0;

    public Color leftColour;
    public Color rightColour;

    public override int[] GetSolidTileIds() { return new int[] { 1 }; }
    public override int[] GetSpawnTileIds() { return new int[] { }; }
    public override void SpawnTileId(int TileId, twin TilePos) { throw new System.NotImplementedException(); }

    public float energyBase = 2;
    public float energyMultiplier = 5;

    public Dictionary<twin, BonsaiNode> nodeDict = new Dictionary<twin, BonsaiNode>();

    [System.NonSerialized] public HashSet<BonsaiNode> activeNodes = new HashSet<BonsaiNode>();
    [System.NonSerialized] public int activeNodeCount;
    [System.NonSerialized] public float energyPerNode;

    public void AddNode(BonsaiNode node)
    {
        activeNodes.Add(node);
        activeNodeCount++;
        energyPerNode = energyBase + energyMultiplier / activeNodeCount;

        nodeDict.Add(node.my_cell_pos, node);
        Sett(node.my_cell_pos, 1); // set it solid.
    }

    public void RetireNode(BonsaiNode node)
    {
        if (activeNodes.Remove(node))
        {
            activeNodeCount--;
            if (activeNodeCount <= 1) energyPerNode = energyMultiplier;
            else energyPerNode = energyBase + energyMultiplier / activeNodeCount;
        }
    }

    public void CutNode(BonsaiNode node)
    {
        Sett(node.my_cell_pos, 0);
        nodeDict.Remove(node.my_cell_pos);

        RetireNode(node);
    }

    private void Start()
    {
        Instance = this;
        InitializeManualTT();
        var firstNode = banks["node"].Spawn<BonsaiNode>(GetEntLot("nodes"));
        firstNode.Setup(mazeMaster, twin.zero);
        AddNode(firstNode);

        banks["cutter"].Spawn(GetEntLot("cutter"));
    }

    private void FixedUpdate()
    {
        var furthestNode = Util.findbest<twin>(nodeDict.Keys, node => {
            var pos = mazeMaster.grid.GetCellCenterWorld(node);
            return Mathf.Max(Mathf.Abs(pos.x) * 2 * 1, pos.y); // * [1] is * [ratio of x:y]. the game will be 1:1
        });
        var furthestPos = mazeMaster.grid.GetCellCenterWorld(furthestNode);
        var desiredOrthographicSize = Mathf.Max(Mathf.Abs(furthestPos.x) * 2 * 1, furthestPos.y) * .5f + 2;
        if (desiredOrthographicSize < 10) desiredOrthographicSize = 10;

        desiredOrthographicSize += Mathf.Sin(Time.time * .23f);

        var desiredOrthographicVelocity = .01f * (desiredOrthographicSize - currentOrthographicSize);
        currentOrthographicVelocity = Util.tow(currentOrthographicVelocity, desiredOrthographicVelocity, .001f);

        currentOrthographicSize += currentOrthographicVelocity;

        gameCamera.orthographicSize = currentOrthographicSize;
        gameCamera.transform.position = new Vector3(0, currentOrthographicSize + 1, -10);

        gameCamera.backgroundColor = Color.Lerp(leftColour, rightColour, Mathf.Sin(Time.time * .10f) * .5f + .5f);
    }

    public BonsaiNode SpawnChildNode(BonsaiNode parent, twin verifiedDir)
    {
        var childNode = banks["node"].Spawn<BonsaiNode>(GetEntLot("nodes"));
        childNode.Setup(mazeMaster, parent.my_cell_pos + verifiedDir);
        childNode.SetParent(parent);
        AddNode(childNode);
        return childNode;
    }
}
