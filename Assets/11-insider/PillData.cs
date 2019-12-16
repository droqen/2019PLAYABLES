using UnityEngine;
using System.Collections;

public class PillData : MonoBehaviour
{
    public bool mode_bullet = true;
    public bool mode_medicine = false;
    public float medicine_energize = 0f;
    [HideInInspector] public Vector2 bullet_velocity;
}
