using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeAbility : CharacterAbility
{
    // set the ability type
    public override AbilityType AbilityBaseMechanic { get { return _abilityBaseMechanic; } }
    private AbilityType _abilityBaseMechanic = AbilityType.Throwable;

    // Explosion variables
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private float _timeToExplosion = 2f;
    [SerializeField] private float _damage = 30f;
    [SerializeField] private float _explosionForce;
    [SerializeField] private LayerMask _interactableLayers;

    // Throwing variables
    [SerializeField] private float _throwForce = 4f;
    [SerializeField] private Transform _throwOrigin;

    // space for the grenade object
    [SerializeField] private GameObject _grenadePrefab;

    // space to store the most recent grenade that has its fusue lit
    private GameObject _lastFusedGrenade;
    
    // store whether the grenade is being cooked in player hand
    private bool _isCooking = false;

    protected override void Start()
    {
        base.Start();
    }


    public override void UseAbility(bool isPressed)
    {
        // Do not use ability if on cooldown. If not, start the cooldown timer
        if (_abilityOnCooldown) return;
        
        // start cooldown if released/thrown
        if(!isPressed) base.UseAbility();

       

        // apply ability to grenade if applicable
        //if(_hasHitEnemyUpgrade) thrownGrenade.GetComponent<Grenade>().AddAbilityOnEnemyHit(_abilityUpgrade);

        // start the explosion timer
        if (isPressed)
        {
            // Spawn a grenade
            GameObject thrownGrenade = Instantiate(_grenadePrefab);
            thrownGrenade.transform.position = _throwOrigin.position;
            if (_hasUpgrade) thrownGrenade.GetComponent<Grenade>().SetUpgrade(_upgradeObject);
            _lastFusedGrenade = thrownGrenade;

            // dont move grenade if being cooked
            thrownGrenade.GetComponent<Rigidbody>().isKinematic = true;

            // set the grenade to follow the throw position
            _isCooking = true;
            StartCoroutine(FollowThrowPosition(thrownGrenade));


            // light the grenade fuse
            thrownGrenade.GetComponent<Grenade>().LightFuse(_explosionRadius, _timeToExplosion, _damage, _explosionForce, _interactableLayers, gameObject);
            return;
        }


        // throw the ability if the player is currently cooking one
        if (_lastFusedGrenade == null) return;
        _isCooking = false;
        _lastFusedGrenade.GetComponent<Rigidbody>().isKinematic = false;
        _lastFusedGrenade.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * _throwForce, ForceMode.Impulse);

    }


    // set the position of the grenade to be at the throw position while being cooked
    private IEnumerator FollowThrowPosition(GameObject thrownObject)
    {
        while (_isCooking)
        {
            thrownObject.transform.position = _throwOrigin.transform.position;
            yield return null;
        }
    }

    public override void ApplyUpgrade(GameObject upgrade)
    {
        // get a reference to the upgrade applied, and set has upgrade to true
        _upgradeObject = upgrade;
        _hasUpgrade = true;
    }
}
