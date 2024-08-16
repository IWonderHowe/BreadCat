using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChaosCritMuliplierOnBulletCrit : OnBulletCritUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosCritMultiplierOnBulletCrit";


    public override void ApplyCritEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ModifyCritMultiplier(ChaosStack.Stacks * 0.01f);
        Debug.Log("Chaos added to multiplier");
    }

    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(this.gameObject);
    }
}
