using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZombieMovement : MonoBehaviour
{
    private Transform player;
    private Vector2 moveDirection;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {

    }
}
