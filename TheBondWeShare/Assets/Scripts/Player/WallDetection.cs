using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetection : MonoBehaviour
{
    [Header("Wall")] 
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] float _wallRayLength = 0.05f;
    [SerializeField] Vector3 _halfExtends, _wallRaydirection;
    public bool facingWall;

    [Header("Ledge")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _ledgeRayLength = 1.0f;
    [SerializeField] Vector3 _ledgeRayOffset;
    RaycastHit _ledgeHit;
    PlayerMovement _playerMovement;
    Rigidbody _rb;
    public bool hanging;
    private bool _ignore;


    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }


    private void FixedUpdate()
    {
        //wall
        if (Physics.BoxCast(transform.position, _halfExtends, _wallRaydirection, Quaternion.identity, _wallRayLength, _wallLayer))
        {
            if (!facingWall)
            {
                facingWall = true;
            }
        }

        else
        {
            if (facingWall)
            {
                facingWall = false;
            }
        }

        //ledge
        if (_ignore) return;

        if (_rb.velocity.y < 0 && Physics.Raycast(transform.position + _ledgeRayOffset, Vector3.down, out _ledgeHit, _ledgeRayLength, _groundLayer))
        {
            if (!hanging)
            {
                hanging = true;
                _playerMovement.LedgeHang();
            }
        }
    }

    public void Turn()
    {
        _wallRaydirection.x *= -1;
        _ledgeRayOffset.x *= -1;
    }

    public IEnumerator IgnoreLedgesForSeconds(float seconds)
    {
        _ignore = true;
        yield return new WaitForSeconds(seconds);
        _ignore = false;
    }

    public int GetLedgePlatformType()
    {
        return _ledgeHit.transform.GetComponent<Platform>().type;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + _wallRaydirection * _wallRayLength, _halfExtends);
        Debug.DrawRay(transform.position + _ledgeRayOffset, Vector3.down * _ledgeRayLength, Color.yellow);
    }

}
