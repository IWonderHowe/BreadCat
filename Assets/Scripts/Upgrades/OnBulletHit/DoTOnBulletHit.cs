using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnBulletHit : OnBulletHitUpgrade
{
    // set spaces for duration and proportional damage of DoT
    private float _totalDoTTime;
    private float _percentOfBulletDamage;

    // constructor for DoT on bullet hit
    public DoTOnBulletHit(float totalDoTTime, float percentOfBulletDamage)
    {
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfBulletDamage;
    }

    // Apply the DoT to the enemy that was hit by a bullet
    public override void ApplyOnHit(Enemy enemy, GameObject player,  float bulletDamage)
    {
        enemy.AddDoTStack(bulletDamage * _percentOfBulletDamage,  _totalDoTTime);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }

}
