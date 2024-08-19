using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletCrit : OnBulletCritUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnBulletCrit";
    public override string UpgradePatron { get { return _upgradePatron; } }
    private string _upgradePatron = "Chaos";

    public override void ApplyCritEffect(GameObject player)
    {
        ChaosStack.SetHasPerfectAccuracy(true);
        player.GetComponent<PlayerController>().CurrentGun.SetPerfectAccuracy(true);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(this.gameObject);
    }

}
