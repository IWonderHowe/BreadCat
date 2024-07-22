using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnBulletHit : OnBulletHitUpgrade
{
    private float _totalDoTTime;
    private float _percentOfBulletDamage;


    public DoTOnBulletHit(float totalDoTTime, float percentOfBulletDamage)
    {
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfBulletDamage;
    }

    public override void ApplyOnHit(Enemy enemy, GameObject player,  float bulletDamage, bool onCrit)
    {
        enemy.AddDoTStack(bulletDamage * _percentOfBulletDamage,  _totalDoTTime);
    }

}
