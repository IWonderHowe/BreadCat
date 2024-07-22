using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletCrit : OnBulletCritUpgrade
{
    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        ChaosStack.SetHasPerfectAccuracy(true);
        player.GetComponent<PlayerController>().CurrentGun.SetPerfectAccuracy(true);
    }

}
