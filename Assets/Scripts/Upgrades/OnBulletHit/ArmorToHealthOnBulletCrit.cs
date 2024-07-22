using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorToHealthOnBulletCrit : OnBulletCritUpgrade
{
    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        player.GetComponent<PlayerCombat>().ArmorToHealth();
    }
}
