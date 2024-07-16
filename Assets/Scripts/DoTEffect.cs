using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTEffect : MonoBehaviour
{
    private float _damage = 0;
    private float _maxTimeApplied = 0;

    private float _timeLeftActive = 0;
    public float TimeLeftActive => _timeLeftActive;
    public float MaxTimeApplied => _maxTimeApplied;
    public float Damage => _damage;

    private float _timeDamageBegan = 0f;
    public float TimeDamageBegan => _timeDamageBegan;

    private float _damagePerSecond => _damage / _maxTimeApplied;

    [SerializeField] private float _damageTickTimer = 0.5f;


    private Enemy _currentEnemy;

    private List<DoTStack> _totalStacks;

    private void Start()
    {
        _currentEnemy = GetComponent<Enemy>();
    }

    public void ApplyDamageOverTime(float totalDamage)
    {

        StartCoroutine(DamageOverTime(totalDamage));
    }

    // apply damage to the enemy over a set amount of time within a coroutine
    private IEnumerator DamageOverTime(float totalDamage)
    {
        // set the time damage began
        _timeDamageBegan = Time.timeSinceLevelLoad;
        float timeOfEnd = _timeDamageBegan + _maxTimeApplied;

        while(_timeDamageBegan <= timeOfEnd)
        {
            _currentEnemy.TakeDamage(_damagePerSecond * _damageTickTimer);
            yield return new WaitForSeconds(_damageTickTimer);
        }
    }

    public void ResetDamageTimer()
    {
        _timeDamageBegan = 0f;
    }

    
    
}

