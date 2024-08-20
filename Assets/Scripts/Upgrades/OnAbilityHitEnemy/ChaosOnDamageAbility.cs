using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnDamageAbility : OnDamageAbilityUpgrade
{
    public override string UpgradeName{ get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnDamageAbility";
    public override string UpgradePatron { get { return _upgradePatron; } }
    private string _upgradePatron = "Chaos";

    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().Ability1Object.GetComponent<CharacterAbility>().ApplyUpgrade(gameObject);
    }

    public override void InvokeUpgrade(GameObject[] enemiesHit)
    {
        Debug.Log(enemiesHit.Length + " stacks added for enemies hit by nade");
        ChaosStack.AddStacks(enemiesHit.Length);
    }

}
