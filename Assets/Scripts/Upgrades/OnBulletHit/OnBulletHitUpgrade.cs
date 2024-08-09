using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletHitUpgrade : Upgrade
{
    private string _upgradeType = "OnBulletHit";
    public override string UpgradeType { get { return _upgradeType; } }

    

    public abstract void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage);

    public override void ApplyUpgrade(GameObject player)
    {
        _upgradeType = "OnBulletHit";
    }
}
