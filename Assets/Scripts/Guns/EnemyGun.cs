using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    // base stats for the enemy gun
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _baseRateOfFire = 1f;

    // stats for if gun is it is a projectile weapon
    [SerializeField] private bool _usesProjectile = false;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _projectileOrigin;
    [SerializeField] private float _projectileSpeed;

    // debugging
    [SerializeField] private GameObject _debugTarget;

    public void DebugShoot()
    {
        Shoot(_debugTarget);
    }

    public void Shoot(GameObject target)
    {
        if (_usesProjectile)
        {
            ShootProjectile(target);
        }
    }

    private void ShootProjectile(GameObject target)
    {
        // set the projectile to be where the its origin is
        GameObject shotProjectile = Instantiate(_projectile);
        _projectile.transform.position = _projectileOrigin.transform.position;

        // find the direction of the player target to get the normalized vector between the enemy and player
        Vector3 vectorToTarget = (target.transform.position - _projectileOrigin.transform.position).normalized;

        // set the projectile to move towards player at set speed
        shotProjectile.GetComponent<Rigidbody>().velocity = vectorToTarget * _projectileSpeed;
    }
}
