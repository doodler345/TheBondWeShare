using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderDetection : MonoBehaviour
{
    public bool detected;
    private MoveableObject detectedObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            detected = true;
            //detectedObject = other.GetComponent<MoveableObject>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            detected = false;
            detectedObject = null;
        }
    }

    public MoveableObject GetDetectedObject()
    {
        return detectedObject;
    }

}
