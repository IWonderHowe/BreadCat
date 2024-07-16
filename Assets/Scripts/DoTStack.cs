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

    private float _endTime => _timeApplied + _totalDoTTime;
    private float _damagePerSecond => _totalDamage / _totalDoTTime;

    public DoTStack(float totalDamage, float tickTime, float totalDoTTime, Enemy attachedEnemy)
    {
        _totalDamage = totalDamage;
        _timeApplied = Time.timeSinceLevelLoad;
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _attachedEnemy = attachedEnemy;
    }

    public IEnumerator ApplyDamage()
    {
        while(Time.timeSinceLevelLoad <= _endTime)
        {
            _attachedEnemy.TakeDamage(_damagePerSecond * _tickTime);
            yield return new WaitForSeconds(_tickTime);
        }

        _attachedEnemy.EndDoTStack(this);
    }


    public void ResetDoTTimer()
    {
        _timeApplied = Time.timeSinceLevelLoad;
    }
}
