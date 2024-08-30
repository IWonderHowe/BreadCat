using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyBombAbility : CharacterAbility
{
    // set the ability type
    public override AbilityType AbilityBaseMechanic { get { return _abilityBaseMechanic; } }
    private AbilityType _abilityBaseMechanic = AbilityType.Throwable;

    // baby properties
    [SerializeField] private float _impactDamage = 10f;
    [SerializeField] private GameObject _babyBombObject;

    // throw properties
    [SerializeField] private Transform _throwOrigin;
    [SerializeField] private float _throwForce;

    // bone properties
    [SerializeField] private float _boneGravity = 5f;
    [SerializeField] private float _boneDamage = 10f;

    // explosion properties
    [SerializeField] private float _bonesOnExplosion = 10f;

    public override void ApplyUpgrade(GameObject upgrade)
    {
        throw new System.NotImplementedException();
    }

    public override void UseAbility(bool isPressed)
    {
        // dont use the ability if its on cooldown
        if (_abilityOnCooldown) return;
        
        // start the ability cooldown
        base.UseAbility();

        // spawn a baby and set its to the throw origin
        Debug.Log("Instantiate a baby");
        GameObject babyBomb = Instantiate(_babyBombObject);
        babyBomb.transform.position = _throwOrigin.position;

        // throw the baby
        babyBomb.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * _throwForce, ForceMode.Impulse);

    }
}
