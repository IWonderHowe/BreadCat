using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnNonDamageAbility : OnNonDamageAbilityUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnNonDamageAbility";
    public override string UpgradePatron { get { return _upgradePatron; } }
    private string _upgradePatron = "Chaos";



    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().Ability2Object.GetComponent<CharacterAbility>().ApplyUpgrade(gameObject);
    }

    public override void InvokeUpgrade(GameObject player, GameObject enemy)
    {
        Debug.Log("stack added from grapple");
        ChaosStack.AddStacks(1);
    }
}
