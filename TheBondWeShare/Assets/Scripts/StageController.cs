using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField] GameObject _playerSetupPrefab;
    RopeController _ropeController;
    private GameObject _playerSetup;
    [SerializeField] Transform _spawn;

    private void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        _playerSetup = Instantiate(_playerSetupPrefab, _spawn.position, Quaternion.identity);
        _ropeController = _playerSetup.GetComponentInChildren<RopeController>();
    }

    public void ReboundPlayers(Vector3 spawnPos)
    {
        if (_ropeController != null) return;

        _spawn.position = spawnPos;

        DestroyPlayers();
        SpawnPlayers();
    }

    private void DestroyPlayers()
    {
        Destroy(_playerSetup);
    }
}
