using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnReloadUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } }
    private string _upgradeType = "OnReload";

    public abstract void ApplyReloadEffect(GameObject player);


}
