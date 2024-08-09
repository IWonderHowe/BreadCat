using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletHitUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnBulletHit";

    public abstract void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage);
}
