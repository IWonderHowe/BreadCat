using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosPatronRoF : PatronUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosPatronRoF";

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 1;


    [SerializeField] private float _stackMultiplierOnUpgrade = 3f;
    [SerializeField] private float _accuracyPenaltyMultiplier = 2f;
    public override void ApplyUpgrade(GameObject player)
    {
        // make the chaos stacks do this much more per stack
        ChaosStack.ModifyStackMultiplier(_stackMultiplierOnUpgrade);

        // adjust accuracy penalty
        ChaosStack.ModifyStackAccuracyPenalty(_accuracyPenaltyMultiplier);
    }

}
