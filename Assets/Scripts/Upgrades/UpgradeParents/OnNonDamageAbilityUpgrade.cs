using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnNonDamageAbilityUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnNonDamageAbility";

    public abstract void InvokeUpgrade(GameObject player, GameObject enemy);

}
