using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour
{
    // Gun base stats
    [SerializeField] private float _damage = 15f;
    [SerializeField] private float _range = 15f;
    [SerializeField] private float _baseRateOfFire = 1f;
    [SerializeField] private float _baseCritMultiplier = 2f;
    private float _effectiveCritMultiplier;

    // ammo variables
    [SerializeField] private int _magSize = 20;
    [SerializeField] private int _currentAmmo;
    public int MaxAmmo => _magSize;
    public int CurrentAmmo => _currentAmmo;


    // Reloading variables
    [SerializeField] private float _reloadSpeed = 1.5f;
    private bool _isReloading;
    private bool _manualReload;
    public bool IsReloading => _isReloading;
    public bool ManualReload => _manualReload;

    

    // Info for gun
    [SerializeField] private bool _usesProjectile = false;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _player;
    [SerializeField] private LayerMask _shootableLayers;

    // Accuracy variables
    [SerializeField] private float _baseAccuracy = 0.80f;
    [SerializeField] private float _minAccuracySpread = 1f;
    [SerializeField] private float _effectiveAccuracy;


    // expose info in relation to gun and shooting
    public bool IsShooting => _isShooting;
    public bool CanShoot => _canShoot;

    // private spaces to store shooting info (serialized for debugging)
    [SerializeField] private bool _isShooting;
    [SerializeField] private bool _canShoot;
    [SerializeField] private bool _hasPerfectAccuracy;
    private float _timeOfLastShot;

    // variables to hold for the gun
    private float _secondsBetweenShots => 1 / _effectiveRateOfFire;
    private float _effectiveRateOfFire;
    private float _rateOfFireMultiplier = 1f;
    private float _damageMultiplier = 1f;
    
    // space for the bullet tracers
    [SerializeField] private TrailRenderer _trailRenderer;
    private Camera _playerCam => Camera.main;

    // spaces to store gameobjects related to the gun
    [SerializeField] private GameObject _gunMag;
    [SerializeField] private Vector3 _throwablesOrigin;
    [SerializeField] private GameObject _bulletOrigin;
    [SerializeField] private GameObject _gunObject;
    [SerializeField] private float _throwForce;

    // Create an area on the gun that will implement gun upgrades. Using object oriented upgrades, so they can be prefabbed
    [SerializeField] private GameObject _onHitObject;
    [SerializeField] private GameObject _onShotObject;
    [SerializeField] private GameObject _onReloadObject;
    private GameObject _onCrit1Object;
    private GameObject _onCrit2Object;
    private GameObject _onKillObject;

    // store if upgrade slot is filled
    private bool _onCrit1Active = false;
    private bool _onCrit2Active = false;
    private bool _onHitActive = false;
    private bool _onShotActive = false;
    private bool _onReloadActive = false;
    private bool _onKillActive = false;

    public bool OnHitActive => _onHitActive;
    public bool OnShotActive => _onShotActive;
    public bool OnReloadActive => _onReloadActive;

    private UpgradeManager _upgradeManager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _timeOfLastShot = 0f;
    }

    private void Awake()
    {


        // begin the effective rate of fire and bullet spread to be at the base stats
        _effectiveRateOfFire = _baseRateOfFire;
        _effectiveAccuracy = _baseAccuracy;
        _upgradeManager = FindObjectOfType<UpgradeManager>();


        // fill gun to max ammo and allow the player to be able to shoot
        _currentAmmo = _magSize;
        _canShoot = true;

        /* get upgrades from upgrade manager
        _upgradeManager = FindObjectOfType<UpgradeManager>();
        if (_upgradeManager != null && _upgradeManager.CurrentOnBulletHitUpgrade != null)
        {
            _onHitUpgrade = _upgradeManager.CurrentOnBulletHitUpgrade;
            _onHitActive = true;
        }*/


    }

    private void Update()
    {
        // DEBUGGING
        //Debug.Log(ChaosStack.Stacks);



        // update the rate of fire based on any changes to the RoF multiplier
        UpdateRoFMultiplier();

        // Reload the gun if the player isnt currently and 
        if(_currentAmmo == 0 && !_isReloading) TriggerReload();

        // If the gun is able to shoot while shooting (i.e. time has passed to account for RoF), shoot
        if (_isShooting && _timeOfLastShot + _secondsBetweenShots <= Time.timeSinceLevelLoad && _canShoot)
        {
            Shoot();
        }
    }
    
    // shoot one ammo from gun
    public void Shoot()
    {

        // if the player is able to shoot, interrupt the reload
        _isReloading = false;
        _manualReload = false;

        // alter perfect accuracy from chaos stacks

        // shoot a projectile or raycast, based on weapon
        if (_usesProjectile)
        {
            ShootProjectile();
        }
        else
        {
            ShootRayCast();
        }

        // reduce current ammo by 1
        _currentAmmo--;
    }

    public void ApplyUpgrade(GameObject upgrade)
    {
        switch (upgrade.GetComponent<Upgrade>().GetUpgradeType())
        {
            case "OnBulletHit":
                _onHitActive = true;
                _onHitObject = upgrade;
                break;

            case "OnBulletCrit":
                _onCrit1Active = true;
                if (_onCrit1Object == null)
                {
                    _onCrit1Object = upgrade;
                    break;
                }

                _onCrit2Active = true;
                _onCrit2Object = upgrade;
                break;

            case "OnReload":
                _onReloadActive = true;
                _onReloadObject = upgrade;
                break;

            case "OnShot":
                _onShotActive = true;
                _onShotObject = upgrade;
                break;

            case "OnKill":
                _onKillActive = true;
                _onKillObject = upgrade;
                break;

            default:
                Debug.Log("Upgrade application  bugged");
                break;
        }
        

        /*switch (upgrade.GetUpgradeType())
        {
            case "OnBulletHit":
                _onHitActive = true;
                break;

            case "OnBulletCrit":
                _onCritActive = true;
                break;

            case "OnReload":
                _onReloadActive = true;
                break;

            case "OnShot":
                _onShotActive = true;
                break;

            default:
                Debug.Log("UhOh");
                break;
        }*/




    }

    // shoot the raycast of a hitscan bullet
    protected virtual void ShootRayCast()
    {
        // spaces to store potential hits as well as the shot direction
        RaycastHit hit;
        Vector3 shotDirection = GetShotDirection();

        //reset chaos stacks perfect accuracy on a shot
        if (ChaosStack.PerfectAccuracy)
        {
            SetPerfectAccuracy(false);
            ChaosStack.SetHasPerfectAccuracy(false);
        }

        //_debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        

        // check to see if the player hits anything with a raycast based on gun properties
        if (Physics.Raycast(_playerCam.gameObject.transform.position, shotDirection, out hit, _range, _shootableLayers))
        {
            // get on shot effect, apply it to this raycast hit
            if (_onShotActive)
            {
                _onShotObject.GetComponent<OnBulletShotUpgrade>().ApplyOnShotEffect(_player, hit);
            } 


            // do this if the shot hits an enemy
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // store the enemy script of the hit enemy
                Enemy enemyHit = hit.collider.gameObject.GetComponentInParent<Enemy>();

                // update the base damage taken by the enemy
                UpdateDamageMultiplier();
                float damageToTake = _damage * (_damageMultiplier);

                if (hit.collider.gameObject.tag == "EnemyCrit")
                {
                    _effectiveCritMultiplier = _baseCritMultiplier;

                    // apply on crit if active
                    if (_onCrit1Active) _onCrit1Object.GetComponent<OnBulletCritUpgrade>().ApplyCritEffect(_player);
                    if (_onCrit2Active) _onCrit2Object.GetComponent<OnBulletCritUpgrade>().ApplyCritEffect(_player);
                    
                    damageToTake *= _baseCritMultiplier;
                }

                // apply damage to the enemy
                hit.collider.gameObject.GetComponentInParent<Enemy>().TakeDamage(damageToTake);

                // if there is an on hit effect active, apply it
                if (_onHitActive)
                {
                    _onHitObject.GetComponent<OnBulletHitUpgrade>().ApplyOnHit(enemyHit, _player, damageToTake);
                }
            }

            

            // Create a trail to show where the shot actually went (expose recoil)
            TrailRenderer bulletTrail = Instantiate(_trailRenderer, _bulletOrigin.transform.position, Quaternion.identity);
            StartCoroutine(SpawnBulletTrail(bulletTrail, _bulletOrigin.transform.position, hit.point));
        }

        // do this on a miss
        else
        {
            
        }

        // set the time of the last shot to now, for fire rate allowance
        _timeOfLastShot = Time.timeSinceLevelLoad;
        
        //Debug.Log("Shots fired");
    }

    private void UpdateDamageMultiplier()
    {
        _damageMultiplier = 1f;
        if (ChaosStack.AffectsDamage)
        {
            _damageMultiplier += ChaosStack.CurrentChaosMultiplier;
        }
    }

    private void UpdateRoFMultiplier()
    {
        _rateOfFireMultiplier = 1f;
        if (ChaosStack.AffectsRoF)
        {
            _rateOfFireMultiplier += ChaosStack.CurrentChaosMultiplier;
        }
    }

    public void SpawnBulletFrom(Vector3 origin, Vector3 destination)
    {
        TrailRenderer bulletTrail = Instantiate(_trailRenderer, origin, Quaternion.identity);
        StartCoroutine(SpawnBulletTrail(bulletTrail, origin, destination));
        
    }

    public void HitEnemy(GameObject enemy)
    {
        // store the enemy script of the hit enemy
        Enemy enemyHit = enemy.GetComponentInParent<Enemy>();

        // if there is an on hit effect active, apply it
        if (_onHitActive)
        {
            _onHitObject.GetComponent<OnBulletHitUpgrade>().ApplyOnHit(enemyHit, _player, _damage);
        }

        // multiply the damage if hitting a critical weakpoint, as well as crit upgrade effects
        float damageToTake = _damage * (1 + ChaosStack.CurrentChaosMultiplier);
        if (enemy.tag == "EnemyCrit")
        {
            _effectiveCritMultiplier = _baseCritMultiplier;

            // apply on crit if active
            if (_onCrit1Active) _onCrit1Object.GetComponent<OnBulletCritUpgrade>().ApplyCritEffect(_player);
            if (_onCrit2Active) _onCrit2Object.GetComponent<OnBulletCritUpgrade>().ApplyCritEffect(_player);

            damageToTake *= _baseCritMultiplier;
        }

        // apply damage to the enemy
        enemy.GetComponentInParent<Enemy>().TakeDamage(damageToTake);
    }

    public void ModifyClipSize(float magSizeToAdd)
    {

    }

    public void ModifyFireRate(float fireRateToAdd)
    {

    }

    public void ModifyCritMultiplier(float critMultiToAdd)
    {
        _effectiveCritMultiplier += critMultiToAdd;
    }

    
    // create a bullet trail to show bullet spread variance due to recoil
    private IEnumerator SpawnBulletTrail(TrailRenderer trail, Vector3 origin,  Vector3 hitPoint)
    {
        // Set the initial variables for the bullet trail
        float time = 0;

        // if the player has a chaos upgrade, make the tracers random colors
        if (_upgradeManager.PatronsAquired.Contains(Patron.Chaos))
        {
            trail.endColor = GetRandomColor();
            trail.startColor = GetRandomColor();
        }

        // while the trail is up, adjust the trail position to go between where it was shot from and where it hits
        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(origin, hitPoint, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        // end the bullet trail when reaching the point the raycast hit, then destroy the bullet trail
        trail.transform.position = hitPoint;
        Destroy(trail.gameObject, trail.time);
    }

    private Color GetRandomColor()
    {
        return Random.ColorHSV(0, 1, 0.8f, 1, 0.8f, 1);
    }

    protected virtual void ShootProjectile()
    {

    }

    // start the reload coroutine
    public void TriggerReload()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        // set the initial values for the reload
        float time = 0f;
        _isReloading = true;

        // begin reload animation


        // Set player to be unable to shoot if there is no ammo left in mag
        if(_currentAmmo == 0)
        {
            _canShoot = false;
        }

        // Wait for until the end of the reload timer (based on reload speed)
        while (time <= _reloadSpeed && _isReloading)
        {
            time += Time.deltaTime;
            yield return null;
        }

        /* OUTDATED ON RELOAD UPGRADE CODE
        // after reloading, trigger reload upgrade effect if applicable
        if (_onReloadActive)
        {
            _onReloadUpgrade.ApplyOnReloadAreaEffect(5, _player.transform.position);
            _onReloadUpgrade.ThrowableMagReloadEffect(_throwablesOrigin, _throwForce, this);
        }*/
        


        if (_onReloadActive) _onReloadObject.GetComponent<OnReloadUpgrade>().ApplyReloadEffect(this.gameObject);

        // Set the player to be done reloading, and refill the current ammo to the magazine size, then allow the player to shoot
        _isReloading = false;
        _manualReload = false;
        if(time > _reloadSpeed) _currentAmmo = _magSize;
        _canShoot = true;
    }

    //
    // Area for universal methods
    //

    // add bullet spread to the look direction of the player to add recoil to a shot
    private Vector3 GetShotDirection()
    {
        // start with the direction the player is looking
        Vector3 direction = Camera.main.transform.forward;

        // calculate the current effective accuracy if the player doesnt have perfect accuracy currently
        if (_hasPerfectAccuracy) _effectiveAccuracy = 0f;
        else _effectiveAccuracy = _minAccuracySpread - (_baseAccuracy - (ChaosStack.CurrentChaosMultiplier / ChaosStack.MaxStacks));

        // set the direction for the player to shoot to deviate by accuracy
        direction += new Vector3(Random.Range(-_effectiveAccuracy, _effectiveAccuracy), Random.Range(-_effectiveAccuracy, _effectiveAccuracy), Random.Range(-_effectiveAccuracy, _effectiveAccuracy));
        return direction;
    }

    // set whether the player has perfect accuracy
    public void SetPerfectAccuracy(bool perfectAccuracy)
    {
        _hasPerfectAccuracy = perfectAccuracy;
    }  

    // reset the gun accuracy to its base stat
    private void ResetAccuracy()
    {
        _effectiveAccuracy = _baseAccuracy;
    }

    // set the player to be shooting (for automatic weapon functionality)
    public void SetIsShooting(bool isShooting)
    {
        _isShooting = isShooting;
    }

    // set the player reload state
    public void SetIsReloading(bool isReloading)
    {
        _isReloading = isReloading;
    }

    // set whether the player reloaded manually
    public void SetIsManualReloading(bool isManualReloading)
    {
        _manualReload = isManualReloading;
    }

    public void TriggerOnKillUpgrade(GameObject enemyKilled)
    {
        if (_onKillActive)
        {
            _onKillObject.GetComponent<OnKillUpgrade>().ApplyKillEffect(_player, enemyKilled);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_player.transform.position, 5);
        //Gizmos.DrawRay(_debugRay);
    }
}
