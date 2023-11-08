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
    private Vector3 _walkVelocity, _climbVelocity;
    [SerializeField] float _walkSpeed = 5f, _pushPullSpeed = 3f, _climbSpeed = 3f;
    [SerializeField] int _jumpForce = 15;
    [SerializeField] float _hangingJumpBoost = 2;
    [SerializeField] bool _allowDoubleJump;
    int _tmpDirection = 1;
    float _tmpPosX;
    int _playerID;
    bool _doubleJmpPossible;
    bool _ladderClimb, _ladderClimbUP; //murks

    Rigidbody _rb;
    [HideInInspector] public ObiRigidbody obiRB;

    GroundDetection _groundDetection;
    WallDetection _wallDetection;
    MoveableDetection _moveableDetection;
    InteractableDetection _interactableDetection;
    Coroutine _delayedTearingEnable, _obiKinematicsEnable;

    Renderer _renderer;
    Color _initColor;


    private void Awake()
    {
        _playerID = GetComponent<PlayerInput>().playerID;
        _rb = GetComponent<Rigidbody>();
        obiRB = GetComponent<ObiRigidbody>();
        _renderer = GetComponentInChildren<Renderer>();
        _initColor = _renderer.material.color;
        _groundDetection = GetComponentInChildren<GroundDetection>();
        _wallDetection = GetComponentInChildren<WallDetection>();
        _moveableDetection = GetComponentInChildren<MoveableDetection>();
        _interactableDetection = GetComponentInChildren<InteractableDetection>();
    }

    private void Update()
    {
        //Checking for State interruptions
        switch (State)
        {
            case STATE.IDLE:
            case STATE.WALK:
                if (_rb.velocity.y < 0 && !_groundDetection.grounded)
                    ChangeState(STATE.FALL);
                break;

            case STATE.PUSH_OBJECT:
            case STATE.PULL_OBJECT:
                if (_rb.velocity.y < 0 && !_groundDetection.grounded)
                    PushPullObject(false);
                break;

            case STATE.JUMP:
                if (_rb.velocity.y < 0)
                {
                    ChangeState(STATE.FALL);
                }
                break;

            case STATE.FALL:
                if (_groundDetection.grounded)
                {
                    ChangeState(STATE.IDLE);
                }
                else if (_rb.velocity.y > 0 && !_wallDetection.facingWall)
                    ChangeState(STATE.ROPE_HANGING);
                break;

            case STATE.LADDERCLIMB:
                LadderClimb(_climbVelocity);
                if (!_interactableDetection.ladderDetected)
                {
                    _climbVelocity = Vector3.zero;
                    _rb.useGravity = true;
                    ChangeState(STATE.IDLE);
                }
                break;

            case STATE.ROPE_HANGING:
                if (_wallDetection.facingWall)
                    ChangeState(STATE.FALL);
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
        else if (State == STATE.LEGDE_HANGING)
        {
            if (_wallDetection.GetLedgePlatformType() == 0) return;
            else LedgeUnhang();
        }

        ChangeState(STATE.FALL);
    }

    public void Move(int xVelocity)
    {
        if (State == STATE.ANCHOR || State == STATE.LEGDE_HANGING)
        {
            return;
        }

        if (xVelocity == 0)
        {
            if (!(State == STATE.JUMP || State == STATE.FALL || State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT || State == STATE.LADDERCLIMB) && _groundDetection.grounded)
                ChangeState(STATE.IDLE);
            return;
        }
        else
        {
            float speedX = xVelocity;

            CheckForTurn(xVelocity);

            if (!(State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT || State == STATE.ROPE_HANGING) && _wallDetection.facingWall)
            {
                ChangeState(STATE.IDLE);
                return;
            }

            else if (State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT)
            {
                speedX *= _pushPullSpeed;

                float deltaX = transform.position.x - _tmpPosX;
                _moveableObject.PushPull(deltaX);
            }

            else if (State == STATE.LADDERCLIMB)
            {
                speedX *= _climbSpeed;
            }

            else
            {
                if(!(State == STATE.ROPE_HANGING)) ChangeState(STATE.WALK);
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

            if(State == STATE.PULL_OBJECT)
                ChangeState(STATE.PUSH_OBJECT);
            else if (State == STATE.PUSH_OBJECT)
                ChangeState(STATE.PULL_OBJECT);

            _tmpDirection = directionX;
        }
    }

    public void Jump()
    {
        if (!_groundDetection.grounded && !_doubleJmpPossible && !(State == STATE.LEGDE_HANGING)) return;
        else if (State == STATE.ANCHOR || State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT || State == STATE.ROPE_HANGING) return;

        ChangeState(STATE.JUMP);

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
            if(_allowDoubleJump) _doubleJmpPossible = !_doubleJmpPossible;
        }
    }

    void LadderClimb(Vector3 yVelocity)
    {
        _rb.MovePosition(transform.position += yVelocity * Time.deltaTime);
    }

    public void Up(bool keyDown)
    {
        if (keyDown)
        {
            if (_interactableDetection.ladderDetected)
            {
                if (State != STATE.LADDERCLIMB)
                {
                    _rb.useGravity = false;
                    _rb.velocity = Vector3.zero;
                    ChangeState(STATE.LADDERCLIMB);
                }
                if(_climbVelocity.normalized != Vector3.up) _climbVelocity = Vector3.up * _climbSpeed;
            }

            else
                Jump();
        }

        else
        {
            if (State == STATE.LADDERCLIMB) _climbVelocity = Vector3.zero;
        }
    }

    public void Down(bool keyDown)
    {
        if (keyDown)
        {
            if (State == STATE.LEGDE_HANGING)
            {
                ChangeState(STATE.FALL);
                LedgeUnhang();
            }

            else if (State == STATE.LADDERCLIMB)
            {
                if (_climbVelocity.normalized != Vector3.down) _climbVelocity = Vector3.down * _climbSpeed;
            }

            else
                Anchor(keyDown);
        }

        else
        {
            if (State == STATE.LADDERCLIMB) _climbVelocity = Vector3.zero;

            else
                Anchor(keyDown);
        }
    }

    public void PressButton()
    {
        if (!_interactableDetection.buttonDetected) return;
        
        _interactableDetection.GetButton().PressButton(); 
    }

    public void Anchor(bool setAnchored)
    {
        if (setAnchored)
        {
            if (State == STATE.JUMP || State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT || !_groundDetection.grounded || StageController.instance.isUnbound) return;

            ChangeState(STATE.ANCHOR);

            _rb.isKinematic = true;

            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _delayedTearingEnable = StartCoroutine(_ropeController.EnableTearing(false, 0));
            _ropeController.StaticDynamicSwitch(true, _playerID);
            _renderer.material.color = Color.red;
        }

        else if (!setAnchored && State == STATE.ANCHOR)
        {
            ChangeState(STATE.IDLE);

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
        if (State == STATE.LADDERCLIMB) return;

        ChangeState(STATE.LEGDE_HANGING);

        _rb.isKinematic = true;
        
        //if (_obiKinematicsEnable != null) StopCoroutine(_obiKinematicsEnable);
        //obiRB.kinematicForParticles = true;


    }
    void LedgeUnhang()
    {
        StartCoroutine(_wallDetection.IgnoreLedgesForSeconds(0.5f));
        _wallDetection.hanging = false;
        
        _rb.isKinematic = false;
        
        //if (_obiKinematicsEnable != null) StopCoroutine(_obiKinematicsEnable);
        //_obiKinematicsEnable = StartCoroutine(EnableObiKinematics(false, 0.2f));
    }
    IEnumerator EnableObiKinematics(bool isActive, float delay)
    {
        yield return new WaitForSeconds(delay);
        obiRB.kinematicForParticles = isActive;
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
        if (!isGrabbing && (State == STATE.PUSH_OBJECT || State == STATE.PULL_OBJECT))
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
        ChangeState(STATE.PUSH_OBJECT);
    }


    public void ObiKinematicManagement(bool isActive)
    {
        //check if player is inside maxRadius again and stop him
        if (isActive && !_ropeController.limitExeeded)
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);

        if (State == STATE.ROPE_HANGING)
        {
            obiRB.kinematicForParticles = false;
            return;
        }

        if (State == STATE.IDLE)
        {
            obiRB.kinematicForParticles = true;
            return;
        }


        obiRB.kinematicForParticles = isActive;
    }


    private void ChangeState(STATE newState)
    {
        if (State == newState) return;

        //changing away from a state
        switch (State)
        {
            case STATE.IDLE:
                if (_ropeController.limitExeeded) ObiKinematicManagement(false);
                break;
            case STATE.ROPE_HANGING:
                _ropeController.SetMaxPlayerDistance(1);
                ObiKinematicManagement(true);
                break;
            default:
                break;
        }

        State = newState;

        //entering new State
        switch (State)
        {
            case STATE.IDLE:
                ObiKinematicManagement(true);
                break;
            case STATE.ROPE_HANGING:
                _ropeController.SetMaxPlayerDistance(2);
                ObiKinematicManagement(false);
                break;
            default:
                break;
        }
    }

    public enum STATE
    {
        IDLE,
        WALK,
        JUMP,
        LEGDE_HANGING,
        ROPE_HANGING,
        FALL,
        ANCHOR,
        PUSH_OBJECT,
        PULL_OBJECT,
        LADDERCLIMB
    }
}
