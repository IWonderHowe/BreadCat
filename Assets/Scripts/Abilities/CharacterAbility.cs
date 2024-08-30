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

    // space to store the upgrade and wether an upgrade has been aquired
    protected GameObject _upgradeObject;
    protected bool _hasUpgrade = false;

    protected virtual void Start()
    {
        // set the ability to be available
        _abilityOnCooldown = false;
    }

    // method to call the application of this upgrade
    public abstract void ApplyUpgrade(GameObject upgrade);

    // begin the cooldown if the ability is used. virtual to allow for overriding
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

    // a virtual method to stop the movement ability
    public virtual void StopMovementAbility()
    {

    }
}

// types of abilities based on how their targeting works
public enum AbilityType
{
    Throwable,
    PointBased
}
