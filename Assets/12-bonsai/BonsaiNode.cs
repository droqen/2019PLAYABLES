using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using navdi3;
using navdi3.maze;

public class BonsaiNode : MazeBody
{
    bonsaixxi xxi { get { return bonsaixxi.Instance; } }

    public LineRenderer liner;

    public float minSpeed = .02f;
    public float maxSpeed = .08f;
    public int minBranchDelay = 10;
    public int maxBranchDelay = 100;

    public Color leaf;
    public Color branch;

    public float maxOffsetLength = 2.5f;

    public float thickness = 1;

    public bool cut = false;

    [System.NonSerialized] public BonsaiNode parent;
    [System.NonSerialized] public Dictionary<twin, BonsaiNode> children = new Dictionary<twin, BonsaiNode>();
    [System.NonSerialized] public float speed;
    [System.NonSerialized] public float branchDelay;
    [System.NonSerialized] public Vector3 offset;
    [System.NonSerialized] public Color color;
    [System.NonSerialized] public bool retired;

    static twin[] hexCompass = {
        new twin(-1,-1),
        new twin(-1,0),
        new twin(-1,1),

        new twin(0,-1),
        new twin(0,1),

        new twin(1,-1),
        new twin(1,0),
        new twin(1,1),
    };

    private void Start()
    {
        this.speed = Random.Range(minSpeed, maxSpeed);
        this.branchDelay = Random.Range(minBranchDelay, maxBranchDelay);

        if (my_cell_pos.y > 0)
        {
            // 'leaf'
            this.color = leaf;
        }
        else
        {
            this.color = branch;
        }

        this.retired = false;
    }

    public override bool CanMoveTo(twin target_pos)
    {
        if (target_pos.y <= 0) return false; // no growing 'into the ground'
        if (IsSolid(target_pos)) return false;
        var target = master.grid.GetCellCenterLocal(target_pos);
        if ((transform.position - target).sqrMagnitude > 10 * 10) return false;
        return true;
    }

    public bool SpawnNewNode(twin dir)
    {
        if (children.ContainsKey(dir)) return false; // i already have a child.
        if (!CanMoveTo(my_cell_pos + dir)) return false;
        this.children[dir] = xxi.SpawnChildNode(this, dir);
        return true; // yay!
    }

    public void SetParent(BonsaiNode parent)
    {
        this.parent = parent;
        this.transform.position = parent.transform.position;
        liner.startWidth = thickness;
        this.offset = Quaternion.Euler(0, 0, Random.value * 360) * Vector2.right * Random.Range(1,maxOffsetLength);
    }

    private void TryToSpawnBranch()
    {
        if (branchDelay > 0)
        {
            branchDelay -= .5f + .75f * xxi.energyPerNode; // no branch, just growth
        }
        else
        {
            //ChoiceStack<twin> dirs;

            //if (parent == null || parent.my_cell_pos.y == this.my_cell_pos.y)
            //{
            //    dirs = new ChoiceStack<twin>(twin.up, twin.up + twin.left, twin.up + twin.right);
            //}
            //else
            //{
            //    dirs = new ChoiceStack<twin>(twin.up, twin.left, twin.up + twin.left, twin.right, twin.up + twin.right);
            //}

            var dirs = new ChoiceStack<twin>(hexCompass);

            //if (this.my_cell_pos.y > 0)
            //{
            //    dirs.RemoveAll(twin.down);
            //    dirs.RemoveAll(twin.down + twin.left);
            //    dirs.RemoveAll(twin.down + twin.right);
            //}

            //if (parent != null && parent.my_cell_pos.y == this.my_cell_pos.y && my_cell_pos.y > 0)
            //{
            //    dirs.RemoveAll(twin.left);
            //    dirs.RemoveAll(twin.right);
            //}

            if (dirs.GetFirstTrue(SpawnNewNode) == twin.zero)
            {
                // failed? i'm no longer an active branch. bye!
                retired = true;
                xxi.RetireNode(this);
            }
            else
            {
                this.branchDelay = Random.Range(minBranchDelay, maxBranchDelay);
            }
        }
    }

    private void FixedUpdate()
    {
        if (cut)
        {
            if (parent != null)
            {
                // darken, but do nothing else.
                if (thickness < 4 && (parent == null || thickness < parent.thickness))
                {
                    thickness += .03f * speed;
                }

                liner.startWidth = parent.thickness;
                liner.SetPosition(0, parent.transform.position - transform.position);
                liner.startColor = parent.color;

                liner.endColor = color = Color.Lerp(leaf, branch, thickness - 1);
            }
        }
        else
        {

            if (IsWithinDistOfCentered(.5f, offset))
            {
                if (!retired)
                {
                    // stopped moving. can start spawning branches.
                    TryToSpawnBranch();
                }

                if (thickness < 4 && (parent == null || thickness < parent.thickness))
                {
                    thickness += .03f * speed;
                    if (!retired && thickness > 2)
                    {
                        retired = true;
                        xxi.RetireNode(this); // retire pre-emptively if i must
                    }
                }
            }
            else
            {
                // grow length
                var moveSpeed = speed * xxi.energyPerNode;
                transform.position += (ToCentered() + offset).normalized * moveSpeed;
            }

            liner.endWidth = thickness;
            liner.SetPosition(1, Vector3.zero);

            if (parent != null)
            {
                liner.startWidth = parent.thickness;
                liner.SetPosition(0, parent.transform.position - transform.position);
                liner.startColor = parent.color;

                liner.endColor = color = Color.Lerp(leaf, branch, thickness - 1);
            }
            else
            {
                liner.startColor = liner.endColor = color = branch;
            }
        }
    }

    public void Cut()
    {
        if (this.parent == null) return; // can't cut

        cut = true;

        var kids = new HashSet<BonsaiNode>();
        CollectChildrenDeeply(ref kids);

        foreach (var kid in kids) {
            xxi.CutNode(kid);
            kid.cut = true;
            kid.parent = null;
            //Object.Destroy(kid.gameObject);
        }

        this.children.Clear();

        xxi.banks["node-cut-copy"].Spawn<BonsaiCutParent>().Setup(this, kids, (liner.GetPosition(1) - liner.GetPosition(0)).normalized);

        // visually turn me into a stump?
        liner.SetPosition(1, liner.GetPosition(0) * .5f + .5f * liner.GetPosition(1));
        liner.endWidth = liner.startWidth * .5f + liner.endWidth * .5f;
        liner.endColor = Color.Lerp(liner.endColor, liner.startColor, .5f);
        liner.numCapVertices = 0;

        xxi.CutNode(this);
    }

    void CollectChildrenDeeply(ref HashSet<BonsaiNode> kids)
    {
        foreach (var child in children.Values)
        {
            child.CollectChildrenDeeply(ref kids);
            kids.Add(child);
        }
    }
}
