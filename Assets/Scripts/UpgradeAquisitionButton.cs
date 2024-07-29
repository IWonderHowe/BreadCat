using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeAquisitionButton : MonoBehaviour
{
    // get the text on the button so we can set it to the current upgrades title
    private TextMeshProUGUI _buttonText;
    
    // get a reference to the button that will be used
    [SerializeField] private Upgrade _buttonUpgrade;
    public Upgrade ButtonUpgrade => _buttonUpgrade;

    public UnityEvent<Upgrade> OnUpgrade; 

    private void Awake()
    {
        // set the current button text
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // make this button reference a specific upgrade
    public void SetUpgrade(Upgrade upgradeSet)
    {
        _buttonUpgrade = upgradeSet;
        _buttonText.SetText(_buttonUpgrade.ToString());
    }

    public void AquireUpgrade()
    {
        OnUpgrade.Invoke(_buttonUpgrade);
    }

    // DEBUGGING
    public void SeeUpgrade()
    {
        Debug.Log(_buttonUpgrade.ToString());
    }
}
