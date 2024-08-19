using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class ChaosOnReload : OnReloadUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnReload";
    public override string UpgradePatron { get { return _upgradePatron; } }
    private string _upgradePatron = "Chaos";

    public override void ApplyReloadEffect(GameObject player)
    {
        if(player.GetComponent<Gun>().ManualReload)
        {
            ChaosStack.ResetStacks();
        }

        ChaosStack.AddStacks(5);
    }
    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(this.gameObject);
    }
}
