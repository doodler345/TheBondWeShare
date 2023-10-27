using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VCamController : MonoBehaviour
{
    public Transform cameraTarget;
    private CinemachineVirtualCamera _vCam;

    private Transform _p1, _p2;

    private void Update()
    {
        if (_p1 == null) return;

        Vector2 betweenPlayers = _p1.position + 0.5f * (_p2.position - _p1.position);
        cameraTarget.position = betweenPlayers;
    }

    public void SetPlayers(Transform p1, Transform p2)
    { 
        this._p1 = p1;
        this._p2 = p2;
    }
}
