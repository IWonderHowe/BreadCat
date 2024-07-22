using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class DoTSpikeOnBulletCrit : OnBulletHitUpgrade
{
    public DoTSpikeOnBulletCrit()
    {

    }

    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage, bool onCrit)
    {
        float damageToApply = 0f;
        if (!onCrit) return;
        foreach(DoTStack stack in enemy.DoTStacks)
        {
            damageToApply += stack.DoTDamageRemaining;
        }

        enemy.TakeDamage(damageToApply);
    }
}
