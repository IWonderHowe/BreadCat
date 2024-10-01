using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBoostOnMovement : MovementUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosBoostOnMovement";

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 1;


    private PlayerMovement _playerMovement;
    private Gun _playerGun;
    private bool _hasBoost;
    private float _initialMultiplier;

    [SerializeField] private float _speedMultiplier = 3f;

    public override void ApplyUpgrade(GameObject player)
    {
        _hasBoost = true;

        _playerMovement = player.GetComponent<PlayerMovement>();
        _playerGun = player.GetComponent<PlayerController>().CurrentGun;
        _initialMultiplier = ChaosStack.PerStackMultiplier;
    }

    public void Update()
    {
        if (!_hasBoost) return;
        if (_playerMovement.TechMovementSpeed > 0)
        {
            float multiplierMod = (float)(_initialMultiplier * (1 + (_playerMovement.CurrentSpeed / (_playerMovement.SpeedLimit - _playerMovement.BaseSpeed) * _speedMultiplier)));
            ChaosStack.SetStackMultiplier(multiplierMod);
        }
        else
        {
            ChaosStack.SetStackMultiplier(_initialMultiplier);
        }
    }
}
