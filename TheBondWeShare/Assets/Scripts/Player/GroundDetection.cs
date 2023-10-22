using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _rayLength = 1.0f;

    public bool grounded;
    private bool _ignore;

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, _rayLength, _groundLayer))
        {
            if(!grounded)
            {
                grounded = true;
            }
        }

        else
        {
            if (grounded)
            {
                grounded = false;
            }
        }
    }
    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.down * _rayLength, Color.blue);
    }
}
