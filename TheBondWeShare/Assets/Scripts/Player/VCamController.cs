using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VCamController : MonoBehaviour
{
    public Transform playerMid;
    public Transform cameraTarget;
    private CinemachineVirtualCamera _vCam;

    [SerializeField] Vector3 _offset;
    [SerializeField] float _zoomFactor = 1f;

    private Vector2 _currentPlayerDistance;
    private Transform _p1, _p2;

    private void Update()
    {
        if (_p1 == null) return;

        _currentPlayerDistance = _p2.position - _p1.position;
        Vector2 betweenPlayers = (Vector2)_p1.position + 0.5f * _currentPlayerDistance;
        
        playerMid.position = betweenPlayers;
        cameraTarget.position = new Vector3(betweenPlayers.x, betweenPlayers.y, -(_currentPlayerDistance.sqrMagnitude * _zoomFactor)) + _offset;
    }

    public void SetPlayers(Transform p1, Transform p2)
    { 
        this._p1 = p1;
        this._p2 = p2;
    }
}
