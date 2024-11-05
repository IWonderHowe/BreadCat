using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletHit : OnBulletHitUpgrade
{
    // set the upgrade name
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnBulletHit";

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 0;



    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        // when the player is hit, add a stack of chaos
        ChaosStack.AddStacks(1);
    }


    public override void ApplyUpgrade(GameObject player)
    {
        // apply this upgrade to the gun
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(this.gameObject);

    }
}
