using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTOnBulletHit : OnBulletHitUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName => ("DoTOnBulletHit");

    public override Patron UpgradePatron { get { return _upgradePatron; } }
    private Patron _upgradePatron = Patron.DoT;

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 0;


    // set spaces for duration and proportional damage of DoT
    [SerializeField] private float _totalDoTTime = 1.5f;
    [SerializeField] private float _percentOfBulletDamage = 0.3f;

    // constructor for DoT on bullet hit
    public void SetDoTVariables(float totalDoTTime, float percentOfBulletDamage)
    {
        _totalDoTTime = totalDoTTime;
        _percentOfBulletDamage = percentOfBulletDamage;
    }

    // Apply the DoT to the enemy that was hit by a bullet
    public override void ApplyOnHit(Enemy enemy, GameObject player,  float bulletDamage)
    {
        // Add a DoT stack to the current enemy, and begin its damage
        DoTStack DoTApplied = new DoTStack(bulletDamage * _percentOfBulletDamage, _totalDoTTime, enemy);
        StartCoroutine(DoTApplied.ApplyDamage());
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
