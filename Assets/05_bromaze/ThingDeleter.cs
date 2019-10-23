using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingDeleter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BroMazer>() != null) Object.Destroy(collision.gameObject);
    }
}
