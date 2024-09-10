using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosPatronDamage : PatronUpgrade
{
    public override string UpgradeName { get {  return _upgradeName; } }
    private string _upgradeName = "ChaosPatronDamage";

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    [SerializeField] private float _accuracyPenaltyMultiplier = 0.25f;

    public override void ApplyUpgrade(GameObject player)
    {
        // replace choas affecting RoF to affecting damage
        ChaosStack.SetAffectsDamage(true);
        ChaosStack.SetAffectsRoF(false);

        // Reduce accuracy penalty
        ChaosStack.ModifyStackAccuracyPenalty(_accuracyPenaltyMultiplier);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
