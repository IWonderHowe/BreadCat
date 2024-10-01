using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorOnBulletHit : OnBulletHitUpgrade
{
    public override string UpgradeName => throw new System.NotImplementedException();

    public override Patron UpgradePatron => throw new System.NotImplementedException();

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 0;

    public ArmorOnBulletHit() { }

    public override void ApplyOnHit(Enemy enemy, GameObject player, float bulletDamage)
    {
        player.GetComponent<PlayerCombat>().AddArmor(0.5f * bulletDamage);
    }

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
