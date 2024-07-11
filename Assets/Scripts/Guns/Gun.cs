using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool IsShooting => _isShooting;


    private bool _isShooting;
    private float _timeOfLastShot;

    // variables to hold for the gun
    private int _currentAmmo;
    

    private Camera _playerCam;
    private float _secondsBetweenShots => 1 / _rateOfFire;


    private Ray _debugRay;
    [SerializeField] private TrailRenderer _trailRenderer;


    private void Awake()
    {
        _playerCam = Camera.main;
    }

    private void Update()
    {
        if (_isShooting && _timeOfLastShot + _secondsBetweenShots <= Time.timeSinceLevelLoad)
        {
            //Debug.Log("Time of shot" + Time.timeSinceLevelLoad);
            ShootRayCast();
        }
        //Debug.Log(_timeOfLastShot + _secondsBetweenShots);
    }
    // Update is called once per frame
    protected virtual void ShootRayCast()
    {
        RaycastHit hit;
        Vector3 shotDirection = GetShotDirection();

        _debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        if (Physics.Raycast(_playerCam.gameObject.transform.position, shotDirection, out hit, _range)){
            if (hit.collider.gameObject.tag == "Enemy") Debug.Log("hit");
            TrailRenderer bulletTrail = Instantiate(_trailRenderer, _playerCam.gameObject.transform.position, Quaternion.identity);
            StartCoroutine(SpawnBulletTrail(bulletTrail, hit));
        } 
        else
        {
            //Debug.Log("Nope");
        }

        _timeOfLastShot = Time.timeSinceLevelLoad;
        
        Debug.Log("Shots fired");
        
    }

    private IEnumerator SpawnBulletTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
    }

    public virtual void ShootProjectile()
    {

    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_debugRay);
    }
}
