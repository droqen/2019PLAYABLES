using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using navdi3;

public class Cutter : MonoBehaviour
{
    bonsaixxi xxi { get { return bonsaixxi.Instance; } }

    public Camera gameCamera;

    LineRenderer liner { get { return GetComponent<LineRenderer>(); } }

    static twin[] numpadCompass = {
        new twin(-1,-1),
        new twin(-1,0),
        new twin(-1,1),

        twin.zero,
        new twin(0,-1),
        new twin(0,1),

        new twin(1,-1),
        new twin(1,0),
        new twin(1,1),
    };

    // Update is called once per frame
    void Update()
    {
        var viewPortPos = gameCamera.ScreenToViewportPoint(Input.mousePosition);
        if (viewPortPos.x < 0 || viewPortPos.y < 0 || viewPortPos.x >= 1 || viewPortPos.y >= 1)
        {
            liner.enabled = false;
        }
        else
        {

            var target = gameCamera.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            var targetCell = new twin(xxi.mazeMaster.grid.WorldToCell(target));

            HashSet<BonsaiNode> branches = new HashSet<BonsaiNode>();
            foreach (var offset in numpadCompass)
            {
                if (xxi.nodeDict.TryGetValue(targetCell + offset, out var branch))
                {
                    branches.Add(branch);
                }
            }

            if (branches.Count > 0)
            {

                var targetNode = Util.findbest<BonsaiNode>(branches, node =>
                {
                    if (node.parent == null) return float.MinValue;
                    if (node.cut) return float.MinValue;

                    var center = node.transform.position + node.liner.GetPosition(0) * .5f + .5f * node.liner.GetPosition(1);
                    return // negative distance
                    -((center - target).sqrMagnitude);
                });

                liner.enabled = true;

                var targetNodeCenter = targetNode.transform.position + targetNode.liner.GetPosition(0) * .5f + .5f * targetNode.liner.GetPosition(1);
                var perp = Quaternion.Euler(0, 0, 90) * (targetNode.liner.GetPosition(1) - targetNode.liner.GetPosition(0)).normalized;

                liner.SetPositions(new Vector3[] {
                targetNodeCenter + perp * (targetNode.thickness * .5f + .5f),
                targetNodeCenter - perp * (targetNode.thickness * .5f + .5f),
            });

                if (Input.GetMouseButtonDown(0))
                {
                    targetNode.Cut();
                }

            }
            else
            {
                liner.enabled = false;
            }
        }

    }
}
