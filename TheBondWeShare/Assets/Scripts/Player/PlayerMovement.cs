using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class PlayerMovement : MonoBehaviour
{
    public STATE State;
    [SerializeField] RopeController _ropeController;
    [SerializeField] Transform _playerModel;
    private Vector3 _walkVelocity;
    [SerializeField] float _speed = 0.5f;
    [SerializeField] int _jumpForce = 15;
    [SerializeField] float _hangingJumpBoost = 2;
    int _tmpDirection = 1;
    Rigidbody _rb;
    ObiRigidbody _obiRB;

    GroundDetection _groundDetection;
    WallDetection _wallDetection;
    Coroutine _delayedTearingEnable, _obiKinematicsEnable, _ropeIsTearing;

    Renderer _renderer;
    Color _initColor;


    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _initColor = _renderer.material.color;
        _rb = GetComponent<Rigidbody>();
        _obiRB = GetComponent<ObiRigidbody>();
        _groundDetection = GetComponentInChildren<GroundDetection>();
        _wallDetection = GetComponentInChildren<WallDetection>();
    }

    private void Update()
    {
        switch (State)
        {
            case STATE.WALK:
                if (_rb.velocity.y < 0)
                    State = STATE.FALL;
                break;

            case STATE.JUMP:
                if (_rb.velocity.y < 0)
                {
                    State = STATE.FALL;
                }
                break;

            case STATE.FALL:
                if (_groundDetection.grounded)
                    State = STATE.IDLE;
                break;

            default: 
                break;
        }
    }

    public void Move(int xVelocity)
    {
        if (State == STATE.ANCHOR || State == STATE.HANGING)
        {
            return;
        }

        if (xVelocity == 0)
        {
            if (!(State == STATE.JUMP || State == STATE.FALL) && _groundDetection.grounded) 
                State = STATE.IDLE;
            return;
        }
        else
        {
            CheckForTurn(xVelocity);

            if (_wallDetection.facingWall)
            {
                State = STATE.IDLE;
                return;
            }

            State = STATE.WALK;

            _walkVelocity = new Vector3(xVelocity * _speed, 0, 0);
            _rb.MovePosition(transform.position += _walkVelocity * Time.deltaTime);
        }
    }

    private void CheckForTurn(int directionX)
    {
        if (_tmpDirection != directionX)
        {
            _playerModel.LookAt(_playerModel.position + new Vector3(directionX, 0, 0));
            _wallDetection.Turn();
            
            _tmpDirection = directionX;
        }
    }

    public void Jump()
    {
        if ((!_groundDetection.grounded && !(State == STATE.HANGING)) || State == STATE.ANCHOR) return;

        State = STATE.JUMP;

        if (_wallDetection.hanging)
        {
            LedgeUnhang();
            _rb.AddForce(Vector3.up * _jumpForce * _hangingJumpBoost, ForceMode.Impulse);
        }

        else
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
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
        if (setAnchored)
        {
            if (State == STATE.JUMP || !_groundDetection.grounded || StageController.instance.isUnbound) return;

            State = STATE.ANCHOR;

            _rb.isKinematic = true;

            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(false, 0));
            _ropeController.StaticDynamicSwitch(true, playerID);
            _renderer.material.color = Color.red;
        }

        else if (!setAnchored && State == STATE.ANCHOR)
        {
            State = STATE.IDLE;

            _rb.isKinematic = false;

            if (!StageController.instance.isUnbound)
            {
                _ropeController.ResetLength();
                _ropeController.StaticDynamicSwitch(false, playerID);
                _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(true, 0.2f));
            }
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
        StartCoroutine(_wallDetection.IgnoreLedgesForSeconds(0.5f));
        _wallDetection.hanging = false;
        
        _rb.isKinematic = false;
        _obiKinematicsEnable = StartCoroutine(EnableObiKinematics(false, 0.2f));
    }

    public void BoundUnbound()
    {
        if (StageController.instance.isUnbound)
        {
            StageController.instance.ReboundPlayers();
        }
        else
        {
            if (_ropeIsTearing != null) return;
            _ropeIsTearing = (StartCoroutine(_ropeController.CutRope(3)));
        }
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
