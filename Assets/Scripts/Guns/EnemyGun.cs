using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    // base stats for the enemy gun
    [SerializeField] private int _damage = 5;
    [SerializeField] private float _range = 100f;
    [SerializeField] private float _baseRateOfFire = 1f;
    private bool isShooting = false;


    // stats for if gun is it is a projectile weapon
    [SerializeField] private bool _usesProjectile = false;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _projectileOrigin;
    [SerializeField] private float _projectileSpeed;
    

    // projectile pattern variables
    [SerializeField] private Vector2Int _projectileRowsAndColumns = new Vector2Int(1,1);
    [SerializeField] private int _shotsInARow = 1;
    [SerializeField] private float _timeBetweenShots = 0.5f;
    [SerializeField] private float _projectileSpacing;

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
            StartCoroutine(ShootPattern(target));
        }
    }

    private IEnumerator ShootPattern(GameObject target)
    {
        if (!isShooting)
        {
            isShooting = true;
            // for every shot in a row, shoot, then wait for the time between shots and shoot again
            for (int i = 0; i < _shotsInARow; i++)
            {
                GameObject shotProjectile = ShootProjectile(target);
                shotProjectile.GetComponent<Projectile>().SetParentObject(gameObject.GetComponentInParent<EnemyCombat>().gameObject);
                shotProjectile.GetComponent<Projectile>().SetDamage(_damage);

                yield return new WaitForSeconds(_timeBetweenShots);
            }
        }
        
        isShooting = false;
    }
    

    private GameObject ShootProjectile(GameObject target)
    {
        // set the projectile to be where the its origin is
        GameObject shotProjectile = Instantiate(_projectile);

        _projectile.transform.position = _projectileOrigin.transform.position;

        // get all projectile objects and set the parent to be the enemy that shot it
        Projectile[] projectiles = shotProjectile.GetComponentsInChildren<Projectile>();
        foreach(Projectile projectile in projectiles)
        {
            projectile.SetParentObject(this.GetComponentInParent<Enemy>().gameObject);
        }


        // find the direction of the player target to get the normalized vector between the enemy and player
        Vector3 vectorToTarget = (target.transform.position - _projectileOrigin.transform.position).normalized;

        // set the projectile to move towards player at set speed
        Rigidbody[] projectilesBodies = shotProjectile.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody body in projectilesBodies)
        {
            body.velocity = vectorToTarget * _projectileSpeed;
        }
        return shotProjectile;
    }
}
