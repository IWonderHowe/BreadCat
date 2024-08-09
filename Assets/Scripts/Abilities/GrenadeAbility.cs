using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrenadeAbility : CharacterAbility
{
    public override string AbilityBaseMechanic { get { return _abilityBaseMechanic; } }
    private string _abilityBaseMechanic = "Throwable";

    // Explosion variables
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private float _timeToExplosion = 4f;
    [SerializeField] private float _damage = 30f;
    [SerializeField] private LayerMask _damageableLayers;

    // Throwing variables
    [SerializeField] private float _throwForce = 4f;
    [SerializeField] private Transform _throwOrigin;

   
    [SerializeField] private GameObject _grenadePrefab;

    private GameObject _abilityUpgrade;
    private bool _hasUpgrade;

    protected override void Start()
    {
        base.Start();
    }


    public override void UseAbility()
    {
        // Do not use ability if on cooldown. If not, start the cooldown timer
        if (_abilityOnCooldown) return;
        base.UseAbility();

        // Spawn a grenade
        GameObject thrownGrenade = Instantiate(_grenadePrefab);
        thrownGrenade.transform.position = _throwOrigin.position;
        if (_hasUpgrade) thrownGrenade.GetComponent<Grenade>().SetUpgrade(_abilityUpgrade);

        // apply ability to grenade if applicable
        //if(_hasHitEnemyUpgrade) thrownGrenade.GetComponent<Grenade>().AddAbilityOnEnemyHit(_abilityUpgrade);

        // throw the grenade, then start the explosion timer
        thrownGrenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * _throwForce, ForceMode.Impulse);
        thrownGrenade.GetComponent<Grenade>().LightFuse(_explosionRadius, _timeToExplosion, _damage, _damageableLayers);
    }

    public override void ApplyUpgrade(GameObject upgrade)
    {
        // get a reference to the upgrade applied, and set has upgrade to true
        _abilityUpgrade = upgrade;
        _hasUpgrade = true;
    }
}
