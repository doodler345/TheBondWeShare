using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableDetection : MonoBehaviour
{
    public bool detected;
    private MoveableObject detectedObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Moveable")
        {
            detected = true;
            detectedObject = other.GetComponent<MoveableObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Moveable")
        {
            detected = false;
            detectedObject = null;
        }
    }

    public MoveableObject GetDetectedObject()
    {
        return detectedObject;
    }

    public void Turn()
    {
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(-pos.x, pos.y, 0); 
    }

}
