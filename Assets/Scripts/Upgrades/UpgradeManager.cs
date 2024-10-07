using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // make space for any player scripts that may be affected
    [SerializeField] private PlayerCombat _playerCombat;
    [SerializeField] private Gun _playerGun;
    private GameObject _player;

    // make a list of all possible upgrades for the player
    private List<OnBulletHitUpgrade> _onBulletHitUpgrades = new List<OnBulletHitUpgrade>();
    private List<OnBulletCritUpgrade> _onBulletCritUpgrades = new List<OnBulletCritUpgrade>();

    private OnBulletHitUpgrade _currentOnBulletHitUpgrade;
    private OnBulletCritUpgrade _currentOnCritUpgrade;

    private UnityEvent _getUpgrade;

    // properties to hold the buttons that will be used to acquire upgrades
    [SerializeField] private Button _upgradeButton1;
    [SerializeField] private Button _upgradeButton2;
    [SerializeField] private Button _upgradeButton3;
    private UpgradeAquisitionButton _aquisition1;
    private UpgradeAquisitionButton _aquisition2;
    private UpgradeAquisitionButton _aquisition3;


    // a space to store the upgrade UI
    [SerializeField] private Canvas _upgradeUI;

    // lists to find details about current and potential upgrades
    [SerializeField] private List<GameObject> _currentUpgrades;
    [SerializeField] private List<GameObject> _availableUpgrades;
    private List<GameObject> _allUpgrades= new List<GameObject>();
    private List<Patron> _patronsAquired = new List<Patron>();
    public List<Patron> PatronsAquired => _patronsAquired;
    public List<GameObject> CurrentUpgrades => _currentUpgrades;

    // make space for a list of upgrade slots and their availablility
    [SerializeField] private List<string> _upgradeSlots = new List<string>();

    
    private void Awake()
    {

        Upgrade[] upgrades = GetComponentsInChildren<Upgrade>();

        foreach(Upgrade upgrade in upgrades)
        {
            _allUpgrades.Add(upgrade.gameObject);
        }

        _aquisition1 = _upgradeButton1.GetComponent<UpgradeAquisitionButton>();
        _aquisition2 = _upgradeButton2.GetComponent<UpgradeAquisitionButton>();
        _aquisition3 = _upgradeButton3.GetComponent<UpgradeAquisitionButton>();

        // make sure there is only ever one upgrade manager
        GameObject[] objectManagers = GameObject.FindGameObjectsWithTag("UpgradeManager");

        if(objectManagers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        

        // populate the upgrade lists if they are not empty
        FilllAvailableUpgradesList();
        FillUpgradeSlots();

        /* get the UI components
        _upgradeButton1.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[0]);
        _upgradeButton2.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[1]);
        _upgradeButton3.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[2]);
        */
        //_upgradeUI.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    

    // fill the upgrade slots with strings of the types of upgrade slots
    private void FillUpgradeSlots()
    {
        _upgradeSlots.Add("OnBulletHit");
        _upgradeSlots.Add("OnBulletCrit1");
        _upgradeSlots.Add("OnBulletCrit2");
        _upgradeSlots.Add("OnReload");
        _upgradeSlots.Add("OnShot");
        _upgradeSlots.Add("OnKill");
        _upgradeSlots.Add("OnDamageAbility");
        _upgradeSlots.Add("OnNonDamageAbility");
        _upgradeSlots.Add("PatronMod1");
        _upgradeSlots.Add("PatronMod2");
        _upgradeSlots.Add("PatronMod3");
        _upgradeSlots.Add("PatronMod4");
    }

    private IEnumerator DelayedSetUIActive(bool uiActive)
    {
        yield return new WaitForSeconds(1);
        _upgradeUI.gameObject.SetActive(uiActive);
    }

    public void ShowUpgrades()
    {
        
    }

    public void GetPlayerObject(GameObject playerObject)
    {
        _player = playerObject;
    }


    private void FilllAvailableUpgradesList()
    {
        // go through all upgrades
        foreach(GameObject upgrade in _allUpgrades)
        {
            // add the upgrade to available upgrades if it has no dependencies
            if(upgrade.GetComponent<Upgrade>().UpgradeDependencies == 0)
            {
                _availableUpgrades.Add(upgrade);
            }
        }

    }
    
    public void StartPlayerUpgradeScreen(bool invoked)
    {
        // set the ui to be active
        _upgradeUI.gameObject.SetActive(true);

        // get random upgrades for the buttons
        GameObject button1Upgrade = AquireRandomUpgrade();
        GameObject button2Upgrade = AquireRandomUpgrade();
        GameObject button3Upgrade = AquireRandomUpgrade();

        // make sure no buttons show the same upgrade
        while (button1Upgrade.GetComponent<Upgrade>().UpgradeName == button2Upgrade.GetComponent<Upgrade>().UpgradeName) button2Upgrade = AquireRandomUpgrade();
        while (button3Upgrade.GetComponent<Upgrade>().UpgradeName == button1Upgrade.GetComponent<Upgrade>().UpgradeName ||
               button3Upgrade.GetComponent<Upgrade>().UpgradeName == button2Upgrade.GetComponent<Upgrade>().UpgradeName) button3Upgrade = AquireRandomUpgrade();

        // set the selected upgrades to the button
        _aquisition1.SetUpgrade(AquireRandomUpgrade());
        _aquisition2.SetUpgrade(AquireRandomUpgrade());
        _aquisition3.SetUpgrade(AquireRandomUpgrade());


        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0.001f;
        _player.GetComponent<PlayerController>().SetPlayerInputActive(false);
        //_player.GetComponentInChildren<CinemachineVirtualCamera>().get
        
    }

    

    private int PatronModSlotsLeft()
    {
        // establish count at 0
        int count = 0;

        //add one to the count for each patron mod slot
        if (_upgradeSlots.Contains("PatronMod1")) count++;
        if (_upgradeSlots.Contains("PatronMod2")) count++;
        if (_upgradeSlots.Contains("PatronMod3")) count++;
        if (_upgradeSlots.Contains("PatronMod4")) count++;

        // the number of patron mod slots left
        return count;
    }

    private int CritModSlotsLeft()
    {
        int count = 0;

        if (_upgradeSlots.Contains("OnEnemyCrit1")) count++;
        if (_upgradeSlots.Contains("OnEnemyCrit2")) count++;

        return count;
    }

    private GameObject AquireRandomUpgrade()
    {
        int upgradeIndex = UnityEngine.Random.Range(0, _availableUpgrades.Count);

        if (_availableUpgrades.Count <= 0)
        {
            return null;
        }

        
        if(_availableUpgrades.Count > 3)
        {
            /*while(_upgradeButton1.gameObject.GetComponent<UpgradeAquisitionButton>().CurrentUpgrade == _availableUpgrades[upgradeIndex] ||
                  _upgradeButton2.gameObject.GetComponent<UpgradeAquisitionButton>().CurrentUpgrade == _availableUpgrades[upgradeIndex] ||
                  _upgradeButton3.gameObject.GetComponent<UpgradeAquisitionButton>().CurrentUpgrade == _availableUpgrades[upgradeIndex])
            {
            }*/
            upgradeIndex = UnityEngine.Random.Range(0, _availableUpgrades.Count - 1);
        }


        GameObject upgradeObject = _availableUpgrades[upgradeIndex];
        Upgrade upgradeToAquire = _availableUpgrades[upgradeIndex].GetComponent<Upgrade>();
        return upgradeObject;


        //AquireUpgrade(upgradeToAquire.UpgradeType, upgradeToAquire.UpgradeName);

        //Debug.Log("RandomUpgradeAquired");

    }

    private GameObject ReturnPossibleUpgrade()
    {
        // how to do
        // only add primary upgrades (upgrades with no dependencies) to the available upgrades area DONE
        // upon upgrade aquisition, remove same slot upgrades from pool of available upgrades (dont forget patron partner upgrades) DONE
        // add upgrades which depend on aquired upgrade for their own availability



        return null;
    }


    public void AquireUpgrade(GameObject upgradeObject)
    {
        Upgrade upgrade = upgradeObject.GetComponent<Upgrade>();
        string upgradeType = upgrade.GetUpgradeType();
        string name = upgrade.GetComponent<Upgrade>().GetUpgradeName();
                
        
        RemoveTypeAvailable(upgradeType);

        AddUpgradeToLists(name);

        // remove all upgrades of this type from available upgrades
        foreach(GameObject upgradeOfType in _availableUpgrades)
        {
            // if the upgrade type is a patron mod and there are slots left, do not remove upgrades of that type from the list
            if(upgradeType == "PatronMod")
            {
                if (PatronModSlotsLeft() > 0) break;
            }

            // only remove upgrade if it is of same type of this upgrades name
            if (upgradeOfType.GetComponent<Upgrade>().GetUpgradeType() == upgradeType)
            {
                _availableUpgrades.Remove(upgradeOfType);
                break;
            }
        }

        // get the amount of this patron base upgrades the player has already
        int playerUpgradesOfPatron = 1;
        foreach (Upgrade playerCurrentUpgrade in _playerCombat.AqcuiredUpgrades)
        {
            if (playerCurrentUpgrade.UpgradeDependencies == 0 && playerCurrentUpgrade.UpgradePatron == upgrade.UpgradePatron) playerUpgradesOfPatron++;
        }

        // add upgrades dependent on this one into available upgrades
        foreach (GameObject potentialDependent in _allUpgrades)
        {
            Upgrade dependentUpgrade = potentialDependent.GetComponent<Upgrade>();

            // ignore if not of type or requires no upgrade
            if (dependentUpgrade.UpgradePatron != upgrade.UpgradePatron || dependentUpgrade.UpgradeDependencies == 0) continue;
            Debug.Log("potential dependent");

            // add upgrades if player has the required base upgrades and it isnt already in the available upgrades
            if (dependentUpgrade.UpgradeDependencies <= playerUpgradesOfPatron && !_playerCombat.AqcuiredUpgrades.Contains(dependentUpgrade))
            {
                Debug.Log("dependent added");
                _availableUpgrades.Add(potentialDependent);
            }
        }



        // reload the scene after getting upgrade
        // TODO: change this if we want to do different levels for milestone
        _player.GetComponent<PlayerController>().SetPlayerInputActive(true);
        SceneManager.LoadScene(0);

        // turn off the ui and start time if stopped
        if (Time.timeScale != 1) Time.timeScale = 1;
        
        _player.GetComponent<PlayerController>().ResetPlayer();
        _upgradeUI.gameObject.SetActive(false);

        Debug.Log(_patronsAquired.Count);


        //_playerGun.ApplyUpgrade();



        /* NEEDS UPGRADE IN CALL 
        Type upgradeType = upgrade.GetType();
        if(_currentOnBulletHitUpgrade == null)
        {
            if (upgrade.GetType() == typeof(DoTOnBulletHit)) _currentOnBulletHitUpgrade = (DoTOnBulletHit)upgrade;
            else if (upgrade.GetType() == typeof(ArmorOnBulletHit)) _currentOnBulletHitUpgrade = (ArmorOnBulletHit)upgrade;
            else if (upgrade.GetType() == typeof(ChaosOnBulletCrit)) _currentOnBulletHitUpgrade = (ChaosOnBulletCrit)upgrade;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        _upgradeUI.enabled = false;
        PlayerController.SetPlayerInputActive(true);

        Scene scene = SceneManager.GetActiveScene();
        Debug.Log(scene.name);
        SceneManager.LoadScene(scene.name);
        */


    }

    private void RemoveTypeAvailable(string upgradeType)
    {
        // remove this slot for available upgrades
        switch (upgradeType)
        {
            // remove the on enemy crit order by taking away on enemy crit 1 first
            case "OnEnemyCrit":
                if (_upgradeSlots.Contains("OnEnemyCrit1"))
                {
                    _upgradeSlots.Remove("OnEnemyCrit1");
                    break;
                }
                _upgradeSlots.Remove("OnEnemyCrit2");
                break;

            // remove patron mods in counting order
            case "PatronMod":
                if (_upgradeSlots.Contains("PatronMod1"))
                {
                    _upgradeSlots.Remove("PatronMod1");
                    break;
                }
                else if (_upgradeSlots.Contains("PatronMod2"))
                {
                    _upgradeSlots.Remove("PatronMod2");
                    break;
                }
                else if (_upgradeSlots.Contains("PatronMod3"))
                {
                    _upgradeSlots.Remove("PatronMod3");
                    break;
                }
                _upgradeSlots.Remove("PatronMod4");
                break;

            // if not a patron or crit upgrade, just remove this upgrade type from the upgrade slots
            default:
                _upgradeSlots.Remove(upgradeType);
                break;
        }
    }

    private void AddUpgradeToLists(string upgradeName)
    {
        // find the upgrade with this name, add it to current upgrades
        foreach (GameObject upgrade in _availableUpgrades)
        {
            if (upgrade.GetComponent<Upgrade>().UpgradeName == upgradeName)
            {
                // add the upgrade to the current upgrades list
                _currentUpgrades.Add(upgrade);

                // if the patron list doesnt have this patron yet, add it to the list
                Patron patron = upgrade.GetComponent<Upgrade>().UpgradePatron;
                Debug.Log(patron);
                if (!_patronsAquired.Contains(patron)) _patronsAquired.Add(patron);

                // apply the upgrade
                upgrade.GetComponent<Upgrade>().ApplyUpgrade(_player);
                break;
            }
        }
    }

}
