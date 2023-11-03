using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayer;
    RaycastHit _hit;

    [SerializeField] float _rayLength = 1.0f;
    [SerializeField] Vector3 _rayOffset;
    public bool grounded;
    private bool _ignore;

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position + _rayOffset, Vector3.down, out _hit, _rayLength, _groundLayer) || Physics.Raycast(transform.position - _rayOffset, Vector3.down, out _hit, _rayLength, _groundLayer))
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

    public int GetPlatformType()
    {
        return _hit.transform.GetComponent<Platform>().type;
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + _rayOffset, Vector3.down * _rayLength, Color.blue);
        Debug.DrawRay(transform.position - _rayOffset, Vector3.down * _rayLength, Color.blue);
    }
}
