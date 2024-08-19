using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    // store the player object
    [SerializeField] private GameObject _player;
    private PlayerController _playerController;
    private PlayerCombat _playerCombat;

    // ammo counter variables
    [SerializeField] private TextMeshProUGUI _ammoCounter;
    [SerializeField] private TextMeshProUGUI _chaosCounter;
    private Gun _currentGun => _playerController.CurrentGun;

    // player health bar variables
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _armorSlider;
    private float _maxHealth => _playerCombat.MaxHealth;
    private float _currentHealth => _playerCombat.CurrentHealth;
    private float _currentArmor => _playerCombat.ArmoredHealth;

    private void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
        _playerCombat = _player.GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        UpdateAmmoCount();
        UpdatePlayerHealth();
        UpdateChaosCounter();
    }

    private void UpdateAmmoCount()
    {
        _ammoCounter.SetText(_currentGun.CurrentAmmo + "/" + _currentGun.MaxAmmo);
    }

    private void UpdatePlayerHealth()
    {
        _armorSlider.value = _currentHealth / _maxHealth;
        _healthSlider.value = (_currentHealth / _maxHealth) - (_currentArmor / _maxHealth);
    }

    private void UpdateChaosCounter()
    {
        _chaosCounter.SetText("Chaos at: " + ChaosStack.Stacks);
    }
}
