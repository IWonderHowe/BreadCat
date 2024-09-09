using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosPatronDamage : PatronUpgrade
{
    public override string UpgradeName { get {  return _upgradeName; } }
    private string _upgradeName = "ChaosPatronDamage";

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override void ApplyUpgrade(GameObject player)
    {
        // replace choas affecting RoF to affecting damage
        ChaosStack.SetAffectsDamage(true);
        ChaosStack.SetAffectsRoF(false);

        // add multiplier to 
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
