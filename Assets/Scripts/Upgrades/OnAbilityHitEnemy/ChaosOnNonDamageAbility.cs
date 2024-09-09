using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosOnNonDamageAbility : OnNonDamageAbilityUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnNonDamageAbility";
    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;



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
