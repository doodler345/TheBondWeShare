using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject _playerSetupPrefab;

    private void Start()
    {
        Instantiate(_playerSetupPrefab, transform);
    }
}
