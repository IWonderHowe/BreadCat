using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    // make a list of all possible upgrades for the player
    private List<OnBulletHitUpgrade> _onBulletHitUpgrades = new List<OnBulletHitUpgrade>();
    private List<OnBulletCritUpgrade> _onBulletCritUpgrades = new List<OnBulletCritUpgrade>();

    private OnBulletHitUpgrade _currentOnBulletHitUpgrade;
    private GameObject _currentOnCritUpgrade;

    private UnityEvent _getUpgrade;

    // properties to hold the buttons that will be used to acquire upgrades
    [SerializeField] private Button _upgrade1;
    [SerializeField] private Button _upgrade2;
    [SerializeField] private Button _upgrade3;

    // properties to hold the currently aquired upgrades

    private void Awake()
    {
        // make sure there is only ever one upgrade manager
        GameObject[] objectManagers = GameObject.FindGameObjectsWithTag("UpgradeManager");

        if(objectManagers.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        // populate the upgrade lists if they are not empty
        FillUpgradeLists();

        _upgrade1.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[0]);
        _upgrade2.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[1]);
        _upgrade3.GetComponent<UpgradeAquisitionButton>().SetUpgrade(_onBulletHitUpgrades[2]);
    }

    public void ReloadScene()
    {
        Scene sceneToLoad = SceneManager.GetActiveScene();

        SceneManager.LoadScene(sceneToLoad.name);

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


    }

    public void AquireOnBulletHitUpgrade(OnBulletHitUpgrade upgrade)
    {
        _currentOnBulletHitUpgrade = upgrade;
    }

}
