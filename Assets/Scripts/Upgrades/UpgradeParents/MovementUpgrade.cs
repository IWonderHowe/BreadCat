using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementUpgrade : Upgrade
{
    public override string UpgradeType { get { return _upgradeType; } } 
    private string _upgradeType = "MovementUpgrade";

}
