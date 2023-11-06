using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour
{
    public UnityEvent buttonPressed;

    public void PressButton()
    {
        buttonPressed?.Invoke();
    }
}
