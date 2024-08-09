using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnDamageAbilityUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnDamageAbility";

    public abstract void InvokeUpgrade(Collider[] enemiesHit);
}

