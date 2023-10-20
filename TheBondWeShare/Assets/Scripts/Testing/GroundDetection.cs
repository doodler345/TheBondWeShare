using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] RopeController _ropeController;
    [SerializeField] LayerMask _floorLayer;
    [SerializeField] float _rayLength = 1.0f;

    public bool grounded;
    ObiRigidbody _obiRB;

    private void Awake()
    {
        _obiRB = GetComponentInParent<ObiRigidbody>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, _rayLength, _floorLayer))
        {
            if(!grounded)
            {
                grounded = true;
                _ropeController.ResetLength();
                //_obiRB.kinematicForParticles = true;
            }
        }

        else
        {
            if (grounded)
            {
                grounded = false;
                //_obiRB.kinematicForParticles = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, Vector3.down * _rayLength, Color.blue);
    }
}
