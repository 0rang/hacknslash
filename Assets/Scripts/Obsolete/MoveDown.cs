using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger");
    }
}
