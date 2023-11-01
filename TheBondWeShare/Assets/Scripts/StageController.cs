using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] GameObject _playerSetupPrefab;
    [SerializeField] VCamController _vCamController;
    [SerializeField] RopeController _ropeController;
    
    private GameObject _playerSetup, _player1, _player2;
    [SerializeField] Transform _spawn;
    [SerializeField] GameObject world1, world2;

    Coroutine _ropeIsTearing;

    [SerializeField] float _maxReboundDistance = 2;
    public bool isUnbound;

    public static StageController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        SpawnPlayers(_spawn.position, true);
    }

    private void SpawnPlayers(Vector3 position, bool isP1Left)
    {
        _playerSetup = Instantiate(_playerSetupPrefab, position, Quaternion.identity);
        _ropeController = _playerSetup.GetComponentInChildren<RopeController>();

        if (isP1Left)
        {
            _player1 = _playerSetup.transform.GetChild(1).gameObject;
            _player2 = _playerSetup.transform.GetChild(2).gameObject;
        }
        else
        {
            _player1 = _playerSetup.transform.GetChild(2).gameObject;
            _player2 = _playerSetup.transform.GetChild(1).gameObject;
        }
        
        _player1.GetComponent<PlayerInput>().playerID = 0;
        _player2.GetComponent<PlayerInput>().playerID = 1;

        SwitchWorld(true);
        isUnbound = false;
        
        _vCamController.SetPlayers(_player1.transform, _player2.transform);
    }

    public void ReboundPlayers()
    {
        if (!isUnbound) return;
        if (!_player1.GetComponentInChildren<GroundDetection>().grounded || !_player2.GetComponentInChildren<GroundDetection>().grounded) return;

        float distance;
        Vector3 respawnPos;
        Vector3 p1Pos = _player1.transform.position;
        Vector3 p2Pos = _player2.transform.position;

        distance = (p1Pos - p2Pos).sqrMagnitude;
            Debug.Log("Playerdistance sqr: " +  distance);

        if (distance > _maxReboundDistance) return;


        respawnPos = p1Pos + 0.5f * (p2Pos - p1Pos);

        bool p1Left;
        if (_player1.transform.position.x < _player2.transform.position.x) p1Left = true;
        else 
            p1Left = false;

        if (_ropeIsTearing != null) StopCoroutine(_ropeIsTearing);
        _ropeIsTearing = null;

        DestroyPlayers();
        SpawnPlayers(respawnPos, p1Left);
    }

    public void UnboundPlayers()
    {
        if (_ropeIsTearing != null) return;
        _ropeIsTearing = StartCoroutine(_ropeController.CutRope(3));

        _ropeController = null;

        isUnbound = true;
        SwitchWorld(false);
    }

    private void DestroyPlayers()
    {
        Destroy(_playerSetup);
    }


    public void SwitchWorld(bool playersConnected)
    {
        if(playersConnected)
        {
            world1.SetActive(true);
            world2.SetActive(false);
        }
        else
        {
            world1.SetActive(false);
            world2.SetActive(true);
        }

        _player1.GetComponent<PlayerMovement>().SwitchWorld();
        _player2.GetComponent<PlayerMovement>().SwitchWorld();
    }
}
