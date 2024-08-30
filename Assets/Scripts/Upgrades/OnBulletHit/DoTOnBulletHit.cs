using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnBulletHit : OnBulletHitUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName => ("DoTOnBulletHit");

    public override string UpgradePatron { get { return _upgradePatron; } }
    private string _upgradePatron = "DoT";

    // set spaces for duration and proportional damage of DoT
    private float _totalDoTTime;
    private float _percentOfBulletDamage;

    

    // constructor for DoT on bullet hit
    public void SetDoTVariables(float totalDoTTime, float percentOfBulletDamage)
    {
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfBulletDamage;
    }

    // Apply the DoT to the enemy that was hit by a bullet
    public override void ApplyOnHit(Enemy enemy, GameObject player,  float bulletDamage)
    {
        
    }

    private void AddDoTStack(float damage, float totalDoTTime, Enemy enemy)
    {
        // Add a DoT stack to the current enemy, and begin its damage
        DoTStack DoTApplied = new DoTStack(damage, totalDoTTime, enemy);
        StartCoroutine(DoTApplied.ApplyDamage());

        // add the DoT stack to this enemies list of DoT stacks, and add the enemy to the list of enemies affected by DoT
        
        DoTStack.AddEnemyToDoTList(this.gameObject);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }

}
