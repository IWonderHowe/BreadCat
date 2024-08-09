using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // make space for any player scripts that may be affected
    private PlayerCombat _playerCombat;
    [SerializeField] private Gun _playerGun;
    [SerializeField] private GameObject _upgradeHolder;
    [SerializeField] private GameObject _player;

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

    // a space to store the upgrade UI
    [SerializeField] private Canvas _upgradeUI;

    // properties to hold the currently aquired upgrades
    [SerializeField] private List<GameObject> _currentUpgrades;
    [SerializeField] private List<GameObject> _availableUpgrades;

    public List<GameObject> CurrentUpgrades => _currentUpgrades;

    // make space for a list of upgrade slots and their availablility
    [SerializeField] private List<string> _upgradeSlots = new List<string>();

    private void Start()
    {
        // get combat and gun scripts attached to player
        _playerCombat = GetComponent<PlayerCombat>();
        //_playerGun = GetComponent<Gun>();

        // make sure there is only ever one upgrade manager
        GameObject[] objectManagers = GameObject.FindGameObjectsWithTag("UpgradeManager");

        if(objectManagers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        // populate the upgrade lists if they are not empty
        FillUpgradeLists();
        FillUpgradeSlots();

        /* get the UI components
        _upgradeButton1.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[0]);
        _upgradeButton2.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[1]);
        _upgradeButton3.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[2]);
        */
        //_upgradeUI.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;
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

    public void ShowUpgrades()
    {
        
    }

    private void FillUpgradeLists()
    {
        //_onBulletHitUpgrades = typeof(OnBulletHitUpgrade);
        _onBulletHitUpgrades.Add(new DoTOnBulletHit(5f, 0.5f));
        _onBulletHitUpgrades.Add(new ArmorOnBulletHit());
        _onBulletHitUpgrades.Add(new ChaosOnBulletHit());



    }

    public static void StartPlayerUpgrade()
    {
        Debug.Log("PlayerHasBeenUpgraded");
        //_upgradeUI.enabled = true;
        Cursor.lockState = CursorLockMode.Confined;
        PlayerController.SetPlayerInputActive(false);
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

    public void AquireRandomUpgrade()
    {
        int upgradeIndex = UnityEngine.Random.Range(0, _availableUpgrades.Count - 1);

        GameObject upgradeObject = _availableUpgrades[upgradeIndex];
        Upgrade upgradeToAquire = _availableUpgrades[upgradeIndex].GetComponent<Upgrade>();

        AquireUpgrade(upgradeToAquire.UpgradeType, upgradeToAquire.UpgradeName);


    }


    public void AquireUpgrade(string upgradeType, string name)
    {
        // remove this slot for available upgrades
        _upgradeSlots.Remove(upgradeType);



        // find the upgrade with this name, add it to current upgrades
        foreach(GameObject upgrade in _availableUpgrades)
        {
            if (upgrade.GetComponent<Upgrade>().UpgradeName == name)
            {
                // add the upgrade to the current upgrades list
                _currentUpgrades.Add(upgrade);

                // apply the upgrade
                upgrade.GetComponent<Upgrade>().ApplyUpgrade(_player);
                break;
            }
        }

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

}
