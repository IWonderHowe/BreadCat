using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBulletShotDoTRoFIncrease : OnBulletShotUpgrade
{
    public override string UpgradeName => throw new System.NotImplementedException();

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

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyOnShotEffect()
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyOnShotEffect(GameObject player, RaycastHit hit)
    {
        throw new System.NotImplementedException();
    }
}
