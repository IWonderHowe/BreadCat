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

    // variables to hold for the gun
    private int _currentAmmo;


    private Camera _playerCam;



    private Ray _debugRay;


    private void Awake()
    {
        _playerCam = Camera.main;
    }
    // Update is called once per frame
    public virtual void ShootRayCast()
    {
        RaycastHit hit;
        _debugRay = new Ray(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward);
        if (Physics.Raycast(_playerCam.gameObject.transform.position, _playerCam.gameObject.transform.forward, out hit, _range)){
            if (hit.collider.gameObject.tag == "Enemy") Debug.Log("hit");
        }
        else
        {
            Debug.Log("Nope");
        }
    }

    public virtual void ShootProjectile()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_debugRay);
    }
}
