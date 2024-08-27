using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : MonoBehaviour
{
    // variables for ability cooldowns
    [SerializeField] protected float _abilityCooldown = 5f;
    [SerializeField] protected bool _abilityOnCooldown;

    // set space to define the style of ability via keywords
    abstract public AbilityType AbilityBaseMechanic { get; }


    // ability info
    [SerializeField] protected bool _isMovementAbility;
    public bool IsMovementAbility => _isMovementAbility;

    [SerializeField] protected bool _isThrowableBased;
    public bool IsThrowableBased => _isThrowableBased;

    [SerializeField] protected bool _isPointBased;
    public bool IsPointBased => _isPointBased;


    protected virtual void Start()
    {
        // set the ability to be available
        _abilityOnCooldown = false;
    }

    public abstract void ApplyUpgrade(GameObject upgrade);

    public virtual void UseAbility(bool isPressed)
    {
        StartCoroutine(AbilityCooldown());
    }

    public virtual void UseAbility()
    {
        // start the cooldown timer when the ability is used
        StartCoroutine(AbilityCooldown());
    }


    private IEnumerator AbilityCooldown()
    {
        // Set the ability to be on cooldown as well as an inital timer
        float timeSinceUse = 0f;
        _abilityOnCooldown = true;

        // wait for the timer to end (based on ability cooldown time)
        while(timeSinceUse < _abilityCooldown && _abilityOnCooldown != false)
        {
            timeSinceUse += Time.deltaTime;
            yield return null;
        }

        // set the ability to be allowed to be used again
        _abilityOnCooldown = false;
    }

    public virtual void StopMovementAbility()
    {

    }
}

public enum AbilityType
{
    Throwable,
    PointBased
}
