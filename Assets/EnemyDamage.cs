using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float health { get; }

    public void TakeDamage()
    {
        Debug.Log("Die Right Now");
        Destroy(this.gameObject);
    }
}
