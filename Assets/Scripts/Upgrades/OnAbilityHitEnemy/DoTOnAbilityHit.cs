using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnAbilityHit : OnAbilityHitEnemyUpgrade
{
    private float _tickTime;
    private float _totalDoTTime;
    private float _percentOfDamage;

    public DoTOnAbilityHit(float tickTime, float totalDoTTime, float percentOfDamage)
    {
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _percentOfDamage = percentOfDamage;
    }

    public override void ApplyOnAbilityHit(Enemy enemy, float damage)
    {
        enemy.AddDoTStack(damage * _percentOfDamage, _tickTime, _totalDoTTime);
    }
}
