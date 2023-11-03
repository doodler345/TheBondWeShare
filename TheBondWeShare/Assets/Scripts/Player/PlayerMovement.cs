using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEditor.Experimental.GraphView;

public class PlayerMovement : MonoBehaviour
{
    public STATE State;
    [SerializeField] RopeController _ropeController;
    [SerializeField] Transform _playerModel;
    private MoveableObject _moveableObject;
    private Vector3 _walkVelocity;
    [SerializeField] float _walkSpeed = 5f, _pushPullSpeed = 3f;
    [SerializeField] int _jumpForce = 15;
    [SerializeField] float _hangingJumpBoost = 2;
    int _tmpDirection = 1;
    float _tmpPosX;
    int _playerID;
    bool _doubleJmpPossible;

    Rigidbody _rb;
    ObiRigidbody _obiRB;

    GroundDetection _groundDetection;
    WallDetection _wallDetection;
    MoveableDetection _moveableDetection;
    Coroutine _delayedTearingEnable, _obiKinematicsEnable;

    Renderer _renderer;
    Color _initColor;


    private void Awake()
    {
        _playerID = GetComponent<PlayerInput>().playerID;
        _rb = GetComponent<Rigidbody>();
        _obiRB = GetComponent<ObiRigidbody>();
        _renderer = GetComponentInChildren<Renderer>();
        _initColor = _renderer.material.color;
        _groundDetection = GetComponentInChildren<GroundDetection>();
        _wallDetection = GetComponentInChildren<WallDetection>();
        _moveableDetection = GetComponentInChildren<MoveableDetection>();
    }

    private void Update()
    {
        switch (State)
        {
            case STATE.IDLE:
                if (_rb.velocity.y < 0 && !_groundDetection.grounded)
                    State = STATE.FALL;
                break;

            case STATE.WALK:
                if (_rb.velocity.y < 0 && !_groundDetection.grounded)
                    State = STATE.FALL;
                break;

            case STATE.PUSHPULL_OBJECT:
                if (_rb.velocity.y < 0 && !_groundDetection.grounded)
                    PushPullObject(false);
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

    public void WorldSwitch()
    {
        if (State == STATE.ANCHOR)
        {
            if (_groundDetection.GetPlatformType() == 0) return;
            else Anchor(false);
        }
        else if (State == STATE.HANGING)
        {
            if (_wallDetection.GetLedgePlatformType() == 0) return;
            else LedgeUnhang();
        }

        State = STATE.FALL;
    }

    public void Move(int xVelocity)
    {
        if (State == STATE.ANCHOR || State == STATE.HANGING)
        {
            return;
        }

        if (xVelocity == 0)
        {
            if (!(State == STATE.JUMP || State == STATE.FALL || State == STATE.PUSHPULL_OBJECT) && _groundDetection.grounded) 
                State = STATE.IDLE;
            return;
        }
        else
        {
            float speedX = xVelocity;

            CheckForTurn(xVelocity);

            if (!(State == STATE.PUSHPULL_OBJECT) && _wallDetection.facingWall)
            {
                State = STATE.IDLE;
                return;
            }

            else if (State == STATE.PUSHPULL_OBJECT)
            {
                speedX *= _pushPullSpeed;

                float deltaX = transform.position.x - _tmpPosX;
                _moveableObject.PushPull(deltaX);
            }

            else
            {
                State = STATE.WALK;
                speedX *= _walkSpeed;                
            }

            _walkVelocity = new Vector3(speedX, 0, 0);
            _tmpPosX = transform.position.x;
            _rb.MovePosition(transform.position += _walkVelocity * Time.deltaTime);
        }
    }

    private void CheckForTurn(int directionX)
    {
        if (_tmpDirection != directionX)
        {
            _playerModel.LookAt(_playerModel.position + new Vector3(directionX, 0, 0));
            _wallDetection.Turn();
            _moveableDetection.Turn();
            
            _tmpDirection = directionX;
        }
    }

    public void Jump()
    {
        if (!_groundDetection.grounded && !_doubleJmpPossible && !(State == STATE.HANGING)) return;
        else if (State == STATE.ANCHOR || State == STATE.PUSHPULL_OBJECT) return;

        State = STATE.JUMP;

        if (_wallDetection.hanging)
        {
            LedgeUnhang();
            _rb.AddForce(Vector3.up * _jumpForce * _hangingJumpBoost, ForceMode.Impulse);
            _doubleJmpPossible = false;
            return;
        }

        else
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _doubleJmpPossible = !_doubleJmpPossible;
        }
    }

    public void Down(bool setAnchored)
    {
        if (State == STATE.HANGING)
        {
            State = STATE.FALL;
            LedgeUnhang();
        }

        else 
            Anchor(setAnchored);
    }

    public void Anchor(bool setAnchored)
    {
        if (setAnchored)
        {
            if (State == STATE.JUMP || State == STATE.PUSHPULL_OBJECT || !_groundDetection.grounded || StageController.instance.isUnbound) return;

            State = STATE.ANCHOR;

            _rb.isKinematic = true;

            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(false, 0));
            _ropeController.StaticDynamicSwitch(true, _playerID);
            _renderer.material.color = Color.red;
        }

        else if (!setAnchored && State == STATE.ANCHOR)
        {
            State = STATE.IDLE;

            _rb.isKinematic = false;

            if (!StageController.instance.isUnbound)
            {
                _ropeController.ResetLength();
                _ropeController.StaticDynamicSwitch(false, _playerID);
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
    IEnumerator EnableObiKinematics(bool isActive, float delay)
    {
        yield return new WaitForSeconds(delay);
        _obiRB.kinematicForParticles = isActive;
    }


    public void BoundUnbound()
    {
        if (StageController.instance.isUnbound)
        {
            StageController.instance.ReboundPlayers();
        }
        else
        {
            StageController.instance.UnboundPlayers();
        }
    }

    public void PushPullObject(bool isGrabbing)
    {
        if (!isGrabbing && State == STATE.PUSHPULL_OBJECT)
        {
            State = STATE.IDLE;
            _moveableObject.getsMoved = false;
            _moveableObject = null;
            return;
        }

        if (!_moveableDetection.detected || !_groundDetection.grounded || (State == STATE.ANCHOR) ) return;
        _moveableObject = _moveableDetection.GetDetectedObject();

        if (_moveableObject.getsMoved) return;

        _moveableObject.getsMoved = true;
        State = STATE.PUSHPULL_OBJECT;
    }




    public enum STATE
    {
        IDLE,
        WALK,
        JUMP,
        HANGING,
        FALL,
        ANCHOR,
        PUSHPULL_OBJECT
    }
}
