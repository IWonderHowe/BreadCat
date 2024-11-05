using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletShotUpgrade : Upgrade
{
    public override string UpgradeType{ get { return _upgradeType; } }
    private string _upgradeType = "OnShot";

    public abstract void ApplyOnShotEffect();
    public abstract void ApplyOnShotEffect(GameObject player, RaycastHit hit);

    public abstract float GetRoFMultiplier();
}
