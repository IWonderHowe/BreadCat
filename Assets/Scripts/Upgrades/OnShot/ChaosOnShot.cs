using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ChaosOnShot : OnBulletShotUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "ChaosOnShot";

    [SerializeField] private float _ricochetRange = 400f;
    [SerializeField] private LayerMask _ricochetableLayers;
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
        Debug.Log("On shot applied");
        if (hit.collider.gameObject.layer != _ricochetableLayers)
        {
            // make an array for the enemy colliders in range
            Collider[] ricochetPotentialHits;

            // fill list of all enemies in range
            ricochetPotentialHits = Physics.OverlapSphere(player.transform.position, _ricochetRange, _ricochetableLayers);
            Debug.Log(ricochetPotentialHits.Length);

            // if an enemy is found, "ricochet" a shot towards it
            if (ricochetPotentialHits.Length <= 1) 
            {
                player.GetComponent<PlayerController>().CurrentGun.SpawnBulletFrom(hit.point, ricochetPotentialHits[0].transform.position);
                Debug.Log("Enemy hit by ricochet");
            }

        }
    }

    public override float GetRoFMultiplier()
    {
        throw new NotImplementedException();
    }
}
