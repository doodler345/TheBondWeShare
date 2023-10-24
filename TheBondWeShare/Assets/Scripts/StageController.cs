using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] GameObject _playerSetupPrefab;
    RopeController _ropeController;
    private GameObject _playerSetup, _player1, _player2;
    [SerializeField] Transform _spawn;

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
        SpawnPlayers(_spawn.position);
    }

    private void SpawnPlayers(Vector3 position)
    {
        _playerSetup = Instantiate(_playerSetupPrefab, position, Quaternion.identity);
        _player1 = _playerSetup.transform.GetChild(1).gameObject;
        _player2 = _playerSetup.transform.GetChild(2).gameObject;

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

        distance = Vector3.Distance(p1Pos, p2Pos);
            Debug.Log("Playerdistance: " +  distance);

        if (distance > _maxReboundDistance) return;


        respawnPos = p1Pos + 0.5f * (p2Pos - p1Pos);
        DestroyPlayers();
        SpawnPlayers(respawnPos);
    }

    private void DestroyPlayers()
    {
        Destroy(_playerSetup);
    }
}
