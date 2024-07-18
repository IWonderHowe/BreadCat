using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnBulletHitUpgrade
{
    public abstract void ApplyOnHit(Enemy enemy, float bulletDamage);
}