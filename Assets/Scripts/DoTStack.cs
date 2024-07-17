using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTStack
{
    // variables needed when contstructing a DoT Stack
    private float _totalDamage;
    private float _timeApplied;
    private float _tickTime;
    private float _totalDoTTime;
    private Enemy _attachedEnemy;

    // calculate the dps and end time based on other variables
    private float _endTime => _timeApplied + _totalDoTTime;
    private float _damagePerSecond => _totalDamage / _totalDoTTime;

    public static List<Enemy> EnemiesAffected;

    // contructor
    public DoTStack(float totalDamage, float tickTime, float totalDoTTime, Enemy attachedEnemy)
    {
        _totalDamage = totalDamage;
        _timeApplied = Time.timeSinceLevelLoad;
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _attachedEnemy = attachedEnemy;
    }


    // coroutine that applies the DoT damage
    public IEnumerator ApplyDamage()
    {
        // while within the time range of the DoT
        while(Time.timeSinceLevelLoad <= _endTime)
        {
            // damage the enemy based on the DPS and apply it based on the tick time, then wait for the tick time
            _attachedEnemy.TakeDamage(_damagePerSecond * _tickTime);
            yield return new WaitForSeconds(_tickTime);
        }

        // when finished, remove this DoT stack from the enemy it was applied from
        _attachedEnemy.EndDoTStack(this);
    }

    public void AddEnemyToDoTList(Enemy enemy)
    {
        if (EnemiesAffected.Contains(enemy)) return;
        EnemiesAffected.Add(enemy);
    }

    public void RemoveEnemyFromDoTList(Enemy enemy)
    {
        EnemiesAffected.Remove(enemy);
    }

    // reset the DoT timer to keep the DoT stack damaging for another max time
    public void ResetDoTTimer()
    {
        _timeApplied = Time.timeSinceLevelLoad;
    }
}
