using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorToHealthOnBulletCrit : OnBulletCritUpgrade
{
    public override string UpgradeName => throw new System.NotImplementedException();

    public override void ApplyCritEffect(GameObject player)
    {
        player.GetComponent<PlayerCombat>().ArmorToHealth();
    }

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
