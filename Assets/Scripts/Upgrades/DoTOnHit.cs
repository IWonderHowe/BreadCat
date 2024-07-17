using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnHit : OnHitUpgrade
{
    private float _tickTime;
    private float _totalDoTTime;
    private float _percentOfBulletDamage;


    public DoTOnHit(float tickTime, float totalDoTTime, float percentOfbulletDamage)
    {
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfbulletDamage;
    }

    public override void ApplyOnHit(Enemy enemy, float bulletDamage)
    {
        enemy.AddDoTStack(bulletDamage * _percentOfBulletDamage, _tickTime, _totalDoTTime);
        Debug.Log("On hit applied");
    }

}
