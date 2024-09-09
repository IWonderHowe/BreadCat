using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnAbilityHit : OnAbilityUpgrade
{
    public override string UpgradeType => throw new System.NotImplementedException();
    private float _totalDoTTime;
    private float _percentOfDamage;

    public override string UpgradeName => throw new System.NotImplementedException();

    public override Patron UpgradePatron => throw new System.NotImplementedException();

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public DoTOnAbilityHit(float totalDoTTime, float percentOfDamage)
    {
        _totalDoTTime = totalDoTTime;
        _percentOfDamage = percentOfDamage;
    }

    /*public override void ApplyOnAbilityHit(Enemy enemy, float damage)
    {
        enemy.AddDoTStack(damage * _percentOfDamage, _totalDoTTime);
    }*/
}
