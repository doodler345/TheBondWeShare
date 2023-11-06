using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDetection : MonoBehaviour
{
    public bool ladderDetected, buttonDetected;
    private InteractableButton detectedButton;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            ladderDetected = true;
            //detectedObject = other.GetComponent<MoveableObject>();
        }
        else if (other.gameObject.tag == "Button")
        {
            buttonDetected = true;
            detectedButton = other.gameObject.GetComponent<InteractableButton>();   
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            ladderDetected = false;
        }        
        else if (other.gameObject.tag == "Button")
        {
            buttonDetected = false;
            detectedButton = null;
        }
    }

    public InteractableButton GetButton()
    {
        return detectedButton;
    }


}
