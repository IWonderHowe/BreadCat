using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float range = 15f;
    [SerializeField] private float reloadSpeed = 1.5f;
    [SerializeField] private float rateOfFire = 1f;
    [SerializeField] private int magSize = 20;
    [SerializeField] private bool usesProjectile = false;

    [SerializeField] private GameObject projectile;


    // Update is called once per frame
    public virtual void ShootRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range)){
            if (hit.collider.gameObject.tag == "Enemy") Debug.Log("hit");
        }
    }

    public virtual void ShootProjectile()
    {

    }
}
