using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBulletShotDoTRoFIncrease : OnBulletShotUpgrade
{
    private float _multiplierPerEnemy = 0f;
    public float RoFMultiplier => DoTStack.EnemiesAffected.Count * _multiplierPerEnemy;

    public OnBulletShotDoTRoFIncrease(float multiplierPerEnemyAffected)
    {
        _multiplierPerEnemy = multiplierPerEnemyAffected;
    }

    public override float GetRoFMultiplier()
    {
        return RoFMultiplier;
    }
    
}
