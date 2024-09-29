using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTStack
{
    // Necessities to do all DoT functionality
    // Needs to be able to apply stacking DoT to enemies on certain conditions being filled
    // Needs to be able to apply all remaining DoT damage at once
    // Needs to be able to use total stacks to affect RoF
    // Upgrade properly through established system
    


    // variables needed when contstructing a DoT Stack
    private float _totalDamage;
    private float _timeApplied;
    private float _totalDoTTime;
    private Enemy _attachedEnemy;

    // calculate the dps and end time based on other variables
    private float _endTime => _timeApplied + _totalDoTTime;
    private float _damagePerSecond => _totalDamage / _totalDoTTime;

    // exposed variables for DoT effects
    private float _dotTimeLeft => _endTime - Time.timeSinceLevelLoad;
    public float DoTDamageRemaining => _damagePerSecond * _dotTimeLeft;


    // keep track of all enemies with DoT currently affecting them
    public static List<GameObject> EnemiesAffected = new List<GameObject>();

    // contructor
    public DoTStack(float totalDamage,  float totalDoTTime, Enemy attachedEnemy)
    {
        _totalDamage = totalDamage;
        _timeApplied = Time.timeSinceLevelLoad;
        _totalDoTTime = totalDoTTime;
        _attachedEnemy = attachedEnemy;
    }


    // coroutine that applies the DoT damage
    public IEnumerator ApplyDamage()
    {
        // while within the time range of the DoT
        while(Time.timeSinceLevelLoad <= _endTime)
        {
            // apply DoT based on initial bullet damage
            _attachedEnemy.TakeDamage(_damagePerSecond * Time.deltaTime);
            
            yield return null;
        }

        // when finished, remove this DoT stack from the enemy it was applied from
        _attachedEnemy.EndDoTStack(this);
    }

    public static void AddEnemyToDoTList(GameObject enemy)
    {
        if (EnemiesAffected.Contains(enemy)) return;
        EnemiesAffected.Add(enemy);
    }

    public static void RemoveEnemyFromDoTList(GameObject enemy)
    {
        EnemiesAffected.Remove(enemy);
    }

    // reset the DoT timer to keep the DoT stack damaging for another max time
    public void ResetDoTTimer()
    {
        _timeApplied = Time.timeSinceLevelLoad;
    }
}
