using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletShotUpgrade : Upgrade
{
    public override string UpgradeType => throw new System.NotImplementedException();
    public abstract float GetRoFMultiplier();
}
