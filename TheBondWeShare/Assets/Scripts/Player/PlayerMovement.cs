using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] RopeController _ropeController;
    [SerializeField] Transform _playerModel;
    [SerializeField] float _speed = 0.5f;
    int _tmpDirection = 1;
    Rigidbody _rb;

    GroundDetection _groundDetection;
    Coroutine _delayedTearingEnable;

    bool _isAnchored;

    Renderer _renderer;
    Color _initColor;


    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _initColor = _renderer.material.color;
        _rb = GetComponent<Rigidbody>();
        _groundDetection = GetComponentInChildren<GroundDetection>();
    }

    public void Move(int directionX)
    {
        if (_isAnchored) return;

        transform.position += new Vector3(directionX * _speed, 0, 0) * Time.deltaTime;

        if (_tmpDirection != directionX)
        {
            Turn(directionX);
            _tmpDirection = directionX;
        }
    }

    private void Turn(int directionX)
    {
        _playerModel.LookAt(_playerModel.position + new Vector3(directionX, 0, 0));
    }

    public void Jump()
    {
        if (!_groundDetection.grounded || _isAnchored) return;

        _rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
    }

    public void Anchor(bool setAnchored, int playerID)
    {
        if (setAnchored)
        {
            if (!_groundDetection.grounded) return;

            _isAnchored = true;
            _rb.velocity = Vector2.zero;
            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _ropeController.EnableTearing(false);
            _ropeController.StaticDynamicSwitch(true, playerID);
            _renderer.material.color = Color.red;
        }

        else
        {
            _isAnchored = false;
            _ropeController.ResetLength();
            _ropeController.StaticDynamicSwitch(false, playerID);
            _delayedTearingEnable = StartCoroutine(nameof(EnableRopeTearing));
            _renderer.material.color = _initColor;
        }
    }

    IEnumerator EnableRopeTearing()
    {
        yield return new WaitForSeconds(0.5f);
        _ropeController.EnableTearing(true);
    }

    public void Crane(bool up)
    {
        if (_isAnchored)
        {
            _ropeController.Crane(up);
        }
    }
}
