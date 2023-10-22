using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] float _rayLength = 1.0f;
    [SerializeField] Vector3 _offset;
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
        if (_ignore) return;

        if (_rb.velocity.y < 0 && Physics.Raycast(transform.position + _offset, Vector3.down, _rayLength, _groundLayer))
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
        _offset.x *= -1;
    }

    public IEnumerator IgnoreForSeconds(float seconds)
    {
        _ignore = true;
        yield return new WaitForSeconds(seconds);
        _ignore = false;
    }


    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position + _offset, Vector3.down * _rayLength, Color.yellow);
    }
}
