using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeAquisitionButton : MonoBehaviour
{
    [SerializeField] private Upgrade _buttonUpgrade;

    public Upgrade ButtonUpgrade => _buttonUpgrade;

    public void SetUpgrade(Upgrade upgradeSet)
    {
        _buttonUpgrade = upgradeSet;
    }

    public void SeeUpgrade()
    {
        Debug.Log(_buttonUpgrade.ToString());
    }
}
