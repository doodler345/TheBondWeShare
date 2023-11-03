using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    public bool getsMoved;

    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void PushPull(float deltaX)
    {
        Vector3 deltaXPos = new Vector3(deltaX, 0, 0);
        _rb.MovePosition(transform.position += deltaXPos);
    }
}
