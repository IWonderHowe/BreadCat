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
    [SerializeField] private GameObject _buttonUpgrade;
    //public Upgrade ButtonUpgrade => _buttonUpgrade;

    [SerializeField] private GameObjectEvent _upgradeEvent;

    private void Awake()
    {
        // set the current button text
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // make this button reference a specific upgrade
    public void SetUpgrade(GameObject upgradeSet)
    {
        _buttonUpgrade = upgradeSet;
        //_buttonUpgrade = upgradeSet.GetComponent<Upgrade>();
        _buttonText.SetText(_buttonUpgrade.ToString());
        
    }

    public void AquireUpgrade()
    {
        _upgradeEvent.Invoke(_buttonUpgrade);
    }

    // DEBUGGING
    public void SeeUpgrade()
    {
        Debug.Log(_buttonUpgrade.ToString());
    }
}
