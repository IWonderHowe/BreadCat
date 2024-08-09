using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletHit : OnBulletHitUpgrade
{
    // set the upgrade name
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnBulletHit";

    // the on hit 
    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        ChaosStack.AddStack();
    }

    public override void ApplyUpgrade(GameObject player)
    {

        List<Upgrade> playerUpgrades = player.GetComponent<PlayerCombat>().AqcuiredUpgrades;


        ChaosOnBulletCrit chaosOnBulletCrit = new ChaosOnBulletCrit();

        if(!playerUpgrades.Contains(chaosOnBulletCrit))
        {
            player.GetComponent<PlayerCombat>().AqcuiredUpgrades.Add(chaosOnBulletCrit);

        }
    }
}
