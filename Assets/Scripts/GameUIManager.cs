using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private PlayerController _playerController;
    private Gun _currentGun => _playerController.CurrentGun;

    [SerializeField] private TextMeshProUGUI _ammoCounter;

    private void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAmmoCount();
    }

    private void UpdateAmmoCount()
    {
        _ammoCounter.SetText(_currentGun.CurrentAmmo + "/" + _currentGun.MaxAmmo);
    }
}
