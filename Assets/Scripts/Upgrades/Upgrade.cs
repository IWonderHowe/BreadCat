using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    abstract public string UpgradeName { get; }
    abstract public string UpgradeType { get; }
    abstract public Patron UpgradePatron { get; }

    public abstract void ApplyUpgrade(GameObject player);

    public string GetUpgradeName()
    {
        return UpgradeName;
    }

    public string GetUpgradeType()
    {
        return UpgradeType;
    }

}

public enum Patron
{
    Chaos,
    DoT,
    Armor
}
