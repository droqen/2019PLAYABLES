using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonsaiCutParent : MonoBehaviour
{
    public LineRenderer liner;

    Rigidbody2D body { get { return GetComponent<Rigidbody2D>(); } }
    CircleCollider2D circle { get { return GetComponent<CircleCollider2D>(); } }

    public void Setup(BonsaiNode originalNode, HashSet<BonsaiNode> subBonsais, Vector3 pushDir)
    {
        body.mass = 1 + .1f * originalNode.thickness;
        float divide = 2f;
        foreach(var bon in subBonsais)
        {
            body.mass += .1f * bon.thickness;
            bon.liner.sortingOrder = 2;
            circle.offset = circle.offset + (Vector2)(bon.transform.position - transform.position) / divide;
            divide++;
        }
        body.velocity = pushDir * Random.Range(5,10);
        body.angularVelocity = Random.Range(-30f, 30f);

        transform.position = originalNode.transform.position;

        liner.SetPosition(0, originalNode.liner.GetPosition(0) * .5f + .5f * originalNode.liner.GetPosition(1));
        liner.SetPosition(1, originalNode.liner.GetPosition(1));
        liner.startColor = Color.Lerp(originalNode.liner.endColor, originalNode.liner.startColor, .5f);
        liner.endColor = originalNode.liner.endColor;
        liner.startWidth = originalNode.liner.startWidth * .5f + .5f * originalNode.liner.endWidth;
        liner.endWidth = originalNode.liner.endWidth;

        if (subBonsais.Count > 0)
            liner.numCapVertices = 0;
        
        foreach(var subBonsai in subBonsais)
        {
            // all cut bonsais are locked to me!
            subBonsai.transform.SetParent(this.transform, true);
        }
    }
}
