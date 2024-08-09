using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletHit : OnBulletHitUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnBulletHit";

    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        ChaosStack.AddStack();
    }

    public override void ApplyUpgrade(GameObject player)
    {
        base.ApplyUpgrade(player);
        
        // apply this change this upgrades name to the correct name
        _upgradeName = "ChaosOnBulletHit";

        List<Upgrade> playerUpgrades = player.GetComponent<PlayerCombat>().AqcuiredUpgrades;

        // remove this upgrade from the available slots
        player.GetComponent<UpgradeManager>().AquireUpgrade(UpgradeType, UpgradeName);

        ChaosOnBulletCrit chaosOnBulletCrit = new ChaosOnBulletCrit();

        if(!playerUpgrades.Contains(chaosOnBulletCrit))
        {
            player.GetComponent<PlayerCombat>().AqcuiredUpgrades.Add(chaosOnBulletCrit);

        }
    }
}
