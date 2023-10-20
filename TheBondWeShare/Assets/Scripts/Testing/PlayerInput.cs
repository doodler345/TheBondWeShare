using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] RopeController _ropeController;
    [SerializeField] Transform _playerModel;
    [SerializeField] int _playerID;
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

    private void Update()
    {
        switch (_playerID)
        {
            case 0:
                if (Input.GetKey("a"))
                {
                    Move(-1);
                }
                else if (Input.GetKey("d"))
                {
                    Move(1);
                }
                if (Input.GetKeyDown("w"))
                {
                    Jump();
                }
                else if (Input.GetKeyDown("s"))
                {
                    Anchor(true);
                }
                else if (Input.GetKeyUp("s"))
                {
                    Anchor(false);
                }
                if (Input.GetKey("q"))
                {
                    Crane(true);
                }

                if (Input.GetKey("e"))
                {
                    Crane(false);
                }

                break;

            case 1:
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Move(-1);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    Move(1);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Jump();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    Anchor(true);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    Anchor(false);
                }

                if (Input.GetKey("o"))
                {
                    Crane(true);
                }

                if (Input.GetKey("p"))
                {
                    Crane(false);
                }
                break;
            default: 
                break;
        }
    }

    private void Move(int directionX)
    {
        if (_isAnchored) return;

        transform.position += new Vector3(directionX * _speed, 0, 0) * Time.deltaTime;
        
        if(_tmpDirection != directionX)
        {
            Turn(directionX);
            _tmpDirection = directionX;
        }
    }

    private void Turn(int directionX)
    {
        _playerModel.LookAt(_playerModel.position + new Vector3(directionX,0,0));
    }

    private void Jump()
    {
        if (!_groundDetection.grounded || _isAnchored) return; 
        
        _rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
    }

    private void Anchor(bool setAnchored)
    {
        if (setAnchored)
        {
            if (!_groundDetection.grounded) return;

            _isAnchored = true;
            _rb.velocity = Vector2.zero;
            if (_delayedTearingEnable != null) StopCoroutine(_delayedTearingEnable);
            _ropeController.EnableTearing(false);
            _ropeController.StaticDynamicSwitch(true, _playerID);
            _renderer.material.color = Color.red;
        }

        else
        {
            _isAnchored = false;
            _ropeController.ResetLength();
            _ropeController.StaticDynamicSwitch(false, _playerID);
            _delayedTearingEnable = StartCoroutine(nameof(EnableRopeTearing));
            _renderer.material.color = _initColor;
        }
    }

    IEnumerator EnableRopeTearing()
    {
        yield return new WaitForSeconds(0.5f);
        _ropeController.EnableTearing(true);
    }

    private void Crane(bool up)
    {
        if (_isAnchored)
        {
            _ropeController.Crane(up);
        }
    }
}
