using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdParkXXI : navdi3.xxi.BaseSimpleXXI
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 32; i++)
        banks["foot bird"].Spawn<FootBird>(GetEntLot("birds"));
    }

    private void LateUpdate()
    {
        SortBirds();
    }

    void SortBirds()
    {
        foreach(Transform birdTransform in GetEntLot("birds").transform)
        {
            birdTransform.position += Vector3.forward * (birdTransform.position.y - birdTransform.position.z);
        }

        //var birds = GetEntLot("birds").GetComponentsInChildren<FootBird>();

        //for (int i = 0; i < birds.Length; i++)
        //{
        //    for (int j = i + 1; j < birds.Length; j++)
        //    {
        //        if (birds[i].transform.position.y < birds[j].transform.position.y)
        //        {
        //            var t = birds[i];
        //            birds[i] = birds[j];
        //            birds[j] = t;
        //        }
        //    }
        //}

        //for (int i = 0; i < birds.Length; i++)
        //{
        //    birds[i].GetComponent<SpriteRenderer>().sortingOrder = 10 + i;
        //}
    }
}
