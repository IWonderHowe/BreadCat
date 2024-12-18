using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChaosOnShot : OnBulletShotUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnShot";
    
    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.Chaos;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 1;


    [SerializeField] private float _ricochetRange = 400f;
    [SerializeField] private LayerMask _ricochetTargetLayers;
    [SerializeField] private float _ricochetPercentChance = 30f;

    public override void ApplyUpgrade(GameObject player)
    {
        player.GetComponent<PlayerController>().CurrentGun.ApplyUpgrade(gameObject);
    }

    public override void ApplyOnShotEffect()
    {
        throw new NotImplementedException();
    }

    public override void ApplyOnShotEffect(GameObject player, RaycastHit hit)
    {

        if(UnityEngine.Random.Range(0, 100) > _ricochetPercentChance)
        {
            return;
        }

        if (hit.collider.gameObject.layer != _ricochetTargetLayers)
        {
            // make an array for the enemy colliders in range
            Collider[] ricochetPotentialHits;

            // fill list of all enemies in range
            ricochetPotentialHits = Physics.OverlapSphere(hit.point, _ricochetRange, _ricochetTargetLayers);
            Debug.Log(ricochetPotentialHits.Length);

            // if an enemy is found, "ricochet" a shot towards it
            if (ricochetPotentialHits.Length >= 1) 
            {
                // store info for target to ricochet to
                GameObject target = ricochetPotentialHits[0].gameObject;
                float closestDistance = _ricochetRange;

                // find the target that is nearest to the player
                foreach(Collider potentialTarget in ricochetPotentialHits)
                {
                    float targetDistance = (potentialTarget.gameObject.transform.position - hit.point).magnitude;
                    
                    // set this to be the nearest target t
                    if ((targetDistance < closestDistance))
                    {
                        target = potentialTarget.gameObject;
                        closestDistance = targetDistance;   
                    }
                }
                

                Gun gun = player.GetComponent<PlayerController>().CurrentGun;
                gun.SpawnBulletFrom(hit.point, target.transform.position);
                gun.HitEnemy(target);
            }

        }
    }

    public override float GetRoFMultiplier()
    {
        throw new NotImplementedException();
    }
}
