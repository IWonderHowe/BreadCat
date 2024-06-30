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

    public bool IsShooting => _isShooting;


    private bool _isShooting;
    private float _timeOfLastShot;

    // variables to hold for the gun
    private int _currentAmmo;
    

    private Camera _playerCam;
    private float _secondsBetweenShots => 1 / _rateOfFire;


    private Ray _debugRay;


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
        _debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        if (Physics.Raycast(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward, out hit, _range)){
            if (hit.collider.gameObject.tag == "Enemy") Debug.Log("hit");
        }
        else
        {
            //Debug.Log("Nope");
        }

        _timeOfLastShot = Time.timeSinceLevelLoad;
        Debug.Log("Shots fired");
        
    }

    public virtual void ShootProjectile()
    {

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
