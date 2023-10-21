using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class PlayerMovement : MonoBehaviour
{
    public STATE State;
    [SerializeField] RopeController _ropeController;
    [SerializeField] Transform _playerModel;
    [SerializeField] float _speed = 0.5f;
    [SerializeField] int _jumpForce = 15;
    [SerializeField] float _hangingJumpBoost = 2;
    int _tmpDirection = 1;
    Rigidbody _rb;
    ObiRigidbody _obiRB;

    GroundDetection _groundDetection;
    LedgeDetection _ledgeDetection;
    Coroutine _delayedTearingEnable, _obiKinematicsEnable;

    Renderer _renderer;
    Color _initColor;


    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _initColor = _renderer.material.color;
        _rb = GetComponent<Rigidbody>();
        _obiRB = GetComponent<ObiRigidbody>();
        _groundDetection = GetComponentInChildren<GroundDetection>();
        _ledgeDetection = GetComponentInChildren<LedgeDetection>();
    }


    public void Move(int directionX)
    {
        if (State == STATE.ANCHOR || State == STATE.HANGING) return;

        if (_rb.velocity.y < 0 && !_groundDetection.grounded)
            State = STATE.FALL;
        else
            State = STATE.WALK;

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
        _ledgeDetection.Turn();
    }

    public void Jump()
    {
        if ((!_groundDetection.grounded && !(State == STATE.HANGING)) || State == STATE.ANCHOR) return;

        State = STATE.JUMP;

        if (_ledgeDetection.hanging)
        {
            LedgeUnhang();
            _rb.AddForce(Vector3.up * _jumpForce * _hangingJumpBoost, ForceMode.Impulse);
        }

        else 
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    public void Down(bool setAnchored, int playerID)
    {
        if (State == STATE.HANGING)
        {
            State = STATE.FALL;
            LedgeUnhang();
        }

        else 
            Anchor(setAnchored, playerID);
    }

    public void Anchor(bool setAnchored, int playerID)
    {
        if (!_groundDetection.grounded) return;

        if (setAnchored)
        {

            State = STATE.ANCHOR;

            _rb.isKinematic = true;

            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(false, 0));
            _ropeController.StaticDynamicSwitch(true, playerID);
            _renderer.material.color = Color.red;
        }

        else if (State == STATE.ANCHOR)
        {
            State = STATE.IDLE;

            _rb.isKinematic = false;

            _ropeController.ResetLength();
            _ropeController.StaticDynamicSwitch(false, playerID);
            _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(true, 0.2f));
            _renderer.material.color = _initColor;
        }
    }

    public void Crane(bool up)
    {
        if (State == STATE.ANCHOR)
        {
            _ropeController.Crane(up);
        }
    }

    public void LedgeHang()
    {
        State = STATE.HANGING;

        _rb.isKinematic = true;
        
        if (_obiKinematicsEnable != null) StopCoroutine(_obiKinematicsEnable);
        _obiKinematicsEnable = StartCoroutine(EnableObiKinematics(true, 0));
        
    }
    void LedgeUnhang()
    {
        StartCoroutine(_ledgeDetection.IgnoreForSeconds(0.5f));
        _ledgeDetection.hanging = false;
        
        _rb.isKinematic = false;
        _obiKinematicsEnable = StartCoroutine(EnableObiKinematics(false, 0.2f));
    }


    IEnumerator EnableObiKinematics(bool isActive, float delay)
    {
        yield return new WaitForSeconds(delay);
        _obiRB.kinematicForParticles = isActive;
    }


    public enum STATE
    {
        IDLE,
        FALL,
        WALK,
        JUMP,
        ANCHOR,
        CRANE,
        HANGING
    }
}
