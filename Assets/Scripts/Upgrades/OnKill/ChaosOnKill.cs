using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnKill : OnKillUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnKill";
    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 1;


    public override void ApplyKillEffect(GameObject player, GameObject enemyKilled)
    {
        ChaosStack.AddMaxStacks(1);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(gameObject);
    }
}
