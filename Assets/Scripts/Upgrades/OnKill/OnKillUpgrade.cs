using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnKillUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnKill";

    public abstract void ApplyKillEffect(GameObject player, GameObject enemyKilled);
}
