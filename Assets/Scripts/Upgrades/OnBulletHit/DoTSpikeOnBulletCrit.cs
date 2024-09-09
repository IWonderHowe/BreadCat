using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTSpikeOnBulletCrit : OnBulletCritUpgrade
{
    public override string UpgradeName => throw new System.NotImplementedException();

    public override Patron UpgradePatron => throw new System.NotImplementedException();

    // contructor (empty) for the DoT spike on crit
    public DoTSpikeOnBulletCrit()
    {

    }

    public override void ApplyCritEffect(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    // check apply the on hit crit effect
    public void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        // set space to store the amount of damage to apply
        float damageToApply = 0f;
        

        // get all damage from DoT on hit enemy and store it in "damage to apply"
        foreach(DoTStack stack in enemy.DoTStacks)
        {
            damageToApply += stack.DoTDamageRemaining;
        }

        // apply the damage to the enemy
        enemy.TakeDamage(damageToApply);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
