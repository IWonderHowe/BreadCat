using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnBulletHit : OnBulletHitUpgrade
{
    private float _tickTime;
    private float _totalDoTTime;
    private float _percentOfBulletDamage;


    public DoTOnBulletHit(float tickTime, float totalDoTTime, float percentOfBulletDamage)
    {
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfBulletDamage;
    }

    public override void ApplyOnHit(Enemy enemy, float bulletDamage)
    {
        enemy.AddDoTStack(bulletDamage * _percentOfBulletDamage, _tickTime, _totalDoTTime);
    }

}
