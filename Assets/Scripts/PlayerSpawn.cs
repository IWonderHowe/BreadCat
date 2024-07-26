using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] GameObject _playerContainer;

    private void Awake()
    {
        Instantiate(_playerContainer, this.transform);
    }
}
