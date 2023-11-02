using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableDetection : MonoBehaviour
{
    public bool isMoveable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Moveable")
        {
            isMoveable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Moveable")
        {
            isMoveable = false;
        }
    }
}
