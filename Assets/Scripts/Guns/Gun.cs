using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{
    // inspector editable gun properties
    [SerializeField] private int _damage = 15;
    [SerializeField] private float _range = 15f;
    [SerializeField] private float _reloadSpeed = 1.5f;
    [SerializeField] private float _rateOfFire = 1f;
    [SerializeField] private int _magSize = 20;
    [SerializeField] private bool _usesProjectile = false;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private float _bulletSpread = 15;

    public bool IsShooting => _isShooting;
    public bool IsReloading => _isReloading;
    public bool CanShoot => _canShoot;

    [SerializeField] private bool _isShooting;
    [SerializeField] private bool _isReloading;
    [SerializeField] private bool _canShoot;

    private float _timeOfLastShot;

    // variables to hold for the gun
    [SerializeField] private int _currentAmmo;
    

    private Camera _playerCam;
    private float _secondsBetweenShots => 1 / _rateOfFire;


    private Ray _debugRay;
    [SerializeField] private TrailRenderer _trailRenderer;


    private void Awake()
    {
        _playerCam = Camera.main;
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
    
    public void Shoot()
    {
        _isReloading = false;

        if (_usesProjectile)
        {
            ShootProjectile();
        }
        else
        {
            ShootRayCast();
        }

        _currentAmmo--;
    }


    protected virtual void ShootRayCast()
    {
        RaycastHit hit;
        Vector3 shotDirection = GetShotDirection();

        _debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        if (Physics.Raycast(_playerCam.gameObject.transform.position, shotDirection, out hit, _range)){
            if (hit.collider.gameObject.tag == "Enemy") hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(_damage);
            TrailRenderer bulletTrail = Instantiate(_trailRenderer, _playerCam.gameObject.transform.position, Quaternion.identity);
            StartCoroutine(SpawnBulletTrail(bulletTrail, hit));
        } 
        else
        {
            
        }

        _timeOfLastShot = Time.timeSinceLevelLoad;
        
        Debug.Log("Shots fired");
        
    }

    private IEnumerator SpawnBulletTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;


        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }

    protected virtual void ShootProjectile()
    {

    }

    public void TriggerReload()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        float time = 0f;
        _isReloading = true;
        // begin reload animation

        // Set player to be unable to shoot if there is no ammo in 
        if(_currentAmmo == 0)
        {
            _canShoot = false;
        }

        while (time <= _reloadSpeed && _isReloading)
        {
            time += Time.deltaTime;
            yield return null;
        }

        _isReloading = false;

        
        if(time > _reloadSpeed) _currentAmmo = _magSize;
        Debug.Log("Reloaded");
        _canShoot = true;
    }

    //
    // Area for universal methods
    //

    // add bullet spread to the an initial direction determined by where the player is looking
    private Vector3 GetShotDirection()
    {
        Vector3 direction = _playerCam.transform.forward;
        direction += new Vector3(Random.Range(-_bulletSpread, _bulletSpread), Random.Range(-_bulletSpread, _bulletSpread), Random.Range(-_bulletSpread, _bulletSpread));
        return direction;
    }

    public void SetIsShooting(bool isShooting)
    {
        _isShooting = isShooting;
    }

    public void SetIsReloading(bool isReloading)
    {
        _isReloading = isReloading;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_debugRay);
    }
}
