using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletCritUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnBulletCrit";

    abstract public void ApplyCritEffect(GameObject player);


}
