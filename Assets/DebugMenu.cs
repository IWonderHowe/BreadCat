using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] private UpgradeManager _upgradeManager;

    [SerializeField] private Button _upgradeScreenButton;
    
    public void GetNewUpgrade()
    {
        _upgradeManager.StartPlayerUpgradeScreen(true);
        Debug.Log("new debug upgrade");
    }

}
