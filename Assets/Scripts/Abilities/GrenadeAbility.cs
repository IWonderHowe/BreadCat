using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAbility : CharacterAbility
{
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private float _timeToExplosion = 4f;
    [SerializeField] private float _damage = 30f;
    [SerializeField] private float _throwForce = 4f;
    [SerializeField] private LayerMask _damageableLayers;

    [SerializeField] private Transform _throwOrigin;
    [SerializeField] private GameObject _grenadePrefab;

    
    public override void UseAbility()
    {
        if (_abilityOnCooldown) return;
        base.UseAbility();
        GameObject thrownGrenade = (GameObject)Instantiate(_grenadePrefab);
        thrownGrenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * _throwForce, ForceMode.Impulse);
        //Debug.Break();
        thrownGrenade.GetComponent<Grenade>().LightFuse(_explosionRadius, _timeToExplosion, _damage, _damageableLayers);
    }
}
