using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    [SerializeField] protected float _abilityCooldown = 5f;

    [SerializeField] protected bool _abilityOnCooldown;

    protected virtual void Start()
    {
        _abilityOnCooldown = false;
    }

    public virtual void UseAbility()
    {
        if (_abilityOnCooldown) return;
        StartCoroutine(AbilityCooldown());
    }

    private IEnumerator AbilityCooldown()
    {
        float timeSinceUse = 0f;
        _abilityOnCooldown = true;

        while(timeSinceUse < _abilityCooldown)
        {
            timeSinceUse += Time.deltaTime;
            yield return null;
        }

        _abilityOnCooldown = false;
    }
}
