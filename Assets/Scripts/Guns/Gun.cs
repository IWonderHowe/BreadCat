using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{
    // inspector editable gun properties
    [SerializeField] private float _damage = 15f;
    [SerializeField] private float _range = 15f;
    [SerializeField] private float _reloadSpeed = 1.5f;
    [SerializeField] private float _rateOfFire = 1f;
    [SerializeField] private int _magSize = 20;
    [SerializeField] private bool _usesProjectile = false;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _bulletSpread = 15;
    [SerializeField] private float _critMultiplier = 2f;

    // expose info in relation to gun and shooting
    public bool IsShooting => _isShooting;
    public bool IsReloading => _isReloading;
    public bool CanShoot => _canShoot;

    // private spaces to store shooting info (serialized for debugging)
    [SerializeField] private bool _isShooting;
    [SerializeField] private bool _isReloading;
    [SerializeField] private bool _canShoot;
    private float _timeOfLastShot;

    // variables to hold for the gun
    [SerializeField] private int _currentAmmo;
    private float _secondsBetweenShots => 1 / _rateOfFire;
    
    // space for the bullet tracers
    [SerializeField] private TrailRenderer _trailRenderer;
    private Camera _playerCam;

    // debug ray
    //private Ray _debugRay;

    // Create an area on the gun that will implement gun upgrades. Using object oriented upgrades, so they can be prefabbed
    [SerializeField] private OnHitUpgrade _onHitUpgrade;
    [SerializeField] private GameObject _onShotUpgrade;
    [SerializeField] private GameObject _onReloadUpgrade;

    private bool _onHitActive = false;
    private bool _onShotActive = false;
    private bool _onReloadActive = false;

    public bool OnHitActive => _onHitActive;
    public bool OnShotActive => _onShotActive;
    public bool OnReloadActive => _onReloadActive;




    private void Awake()
    {
        // Debug area
        _onHitUpgrade = new DoTOnHit(0.5f, 5f, .2f);
        _onHitActive = true;


        // get the players camera
        _playerCam = Camera.main;

        // fill gun to max ammo and allow the player to be able to shoot
        _currentAmmo = _magSize;
        _canShoot = true;
    }

    private void Update()
    {
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


    protected virtual void ShootRayCast()
    {
        // spaces to store potential hits as well as the shot direction
        RaycastHit hit;
        Vector3 shotDirection = GetShotDirection();

        //_debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        
        // check to see if the player hits anything with a raycast based on gun properties
        if (Physics.Raycast(_playerCam.gameObject.transform.position, shotDirection, out hit, _range))
        {
            // do this if the shot hits an enemy
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // if there is an on hit effect active, apply it
                if (_onHitActive) _onHitUpgrade.ApplyOnHit(hit.collider.gameObject.GetComponentInParent<Enemy>(), _damage);
                
                // Apply crit damage if hitting a critical weakpoint, or regular damage if hitting anywhere else on enemy
                float damageToTake = _damage;
                if (hit.collider.gameObject.tag == "EnemyCrit") damageToTake *= _critMultiplier;
                hit.collider.gameObject.GetComponentInParent<Enemy>().TakeDamage(damageToTake);
            }

            // Create a trail to show where the shot actually went (expose recoil)
            TrailRenderer bulletTrail = Instantiate(_trailRenderer, _playerCam.gameObject.transform.position, Quaternion.identity);
            StartCoroutine(SpawnBulletTrail(bulletTrail, hit));
        } 
        else
        {
            
        }

        // set the time of the last shot to now, for fire rate allowance
        _timeOfLastShot = Time.timeSinceLevelLoad;
        
        //Debug.Log("Shots fired");
    }

    // create a bullet trail to show bullet spread variance due to recoil
    private IEnumerator SpawnBulletTrail(TrailRenderer trail, RaycastHit hit)
    {
        // Set the initial variables for the bullet trail
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        // while the trail is up, adjust the trail position to go between where it was shot from and where it hits
        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        // end the bullet trail when reaching the point the raycast hit, then destroy the bullet trail
        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
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

        // Set the player to be done reloading, and refill the current ammo to the magazine size, then allow the player to shoot
        // Debug.Log("Reloaded");
        _isReloading = false;
        if(time > _reloadSpeed) _currentAmmo = _magSize;
        _canShoot = true;
    }

    //
    // Area for universal methods
    //

    // add bullet spread to the look direction of the player to add recoil to a shot
    private Vector3 GetShotDirection()
    {
        Vector3 direction = _playerCam.transform.forward;
        direction += new Vector3(Random.Range(-_bulletSpread, _bulletSpread), Random.Range(-_bulletSpread, _bulletSpread), Random.Range(-_bulletSpread, _bulletSpread));
        return direction;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(_debugRay);
    }
}
