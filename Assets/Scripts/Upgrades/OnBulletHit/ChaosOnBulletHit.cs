using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletHit : OnBulletHitUpgrade
{
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
