using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadeAbility : CharacterAbility
{
    // Explosion variables
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private float _timeToExplosion = 4f;
    [SerializeField] private float _damage = 30f;
    [SerializeField] private LayerMask _damageableLayers;

    // Throwing variables
    [SerializeField] private float _throwForce = 4f;
    [SerializeField] private Transform _throwOrigin;

   
    [SerializeField] private GameObject _grenadePrefab;

    private OnAbilityHitEnemyUpgrade _abilityHitEnemyUpgrade;
    private bool _hasHitEnemyUpgrade = false;

    protected override void Start()
    {
        base.Start();
        _abilityHitEnemyUpgrade = new DoTOnAbilityHit(0.5f, 5f, 0.3f);
        _hasHitEnemyUpgrade = true;
    }


    public override void UseAbility()
    {
        // Do not use ability if on cooldown. If not, start the cooldown timer
        if (_abilityOnCooldown) return;
        base.UseAbility();

        // Spawn a grenade, throw it, then start the explosion timer
        GameObject thrownGrenade = (GameObject)Instantiate(_grenadePrefab, _throwOrigin);
        thrownGrenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * _throwForce, ForceMode.Impulse);
        thrownGrenade.GetComponent<Grenade>().LightFuse(_explosionRadius, _timeToExplosion, _damage, _damageableLayers);
    }
}
