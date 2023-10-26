using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] GameObject _playerSetupPrefab;
    RopeController _ropeController;
    private GameObject _playerSetup, _player1, _player2;
    [SerializeField] Transform _spawn;

    [SerializeField] float _maxReboundDistance = 4;
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

        _ropeController = _playerSetup.GetComponentInChildren<RopeController>();
        isUnbound = false;
    }

    public void ReboundPlayers()
    {
        if (!isUnbound) return;


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

        DestroyPlayers();
        SpawnPlayers(respawnPos, p1Left);
    }

    private void DestroyPlayers()
    {
        Destroy(_playerSetup);
    }
}
