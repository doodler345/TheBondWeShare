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
        Vector2 betweenPlayers = StageController.instance.playerMid.transform.position;
        cameraTarget.position = new Vector3(betweenPlayers.x, betweenPlayers.y, -(_currentPlayerDistance.sqrMagnitude * _zoomFactor)) + _offset;
    }

    public void SetPlayers(Transform p1, Transform p2)
    { 
        this._p1 = p1;
        this._p2 = p2;
    }
}
