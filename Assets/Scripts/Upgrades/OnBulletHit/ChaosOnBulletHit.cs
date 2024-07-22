using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnBulletHit : OnBulletHitUpgrade
{
    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        ChaosStack.AddStack();
    }
}
