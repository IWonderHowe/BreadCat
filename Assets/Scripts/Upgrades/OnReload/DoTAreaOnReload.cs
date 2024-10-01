using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoTAreaOnReload : OnReloadUpgrade
{
    public override string UpgradeName { get { return _upgradeName; } }
    private string _upgradeName = "DoTOnReload";

    public override Patron UpgradePatron => throw new System.NotImplementedException();

    public override int UpgradeDependencies { get { return _upgradeDependencies; } }
    private int _upgradeDependencies = 0;

    public override void ApplyUpgrade(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyReloadEffect(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    /*
    private float _tickTime;
    private float _totalDoTTime;
    private float _damage;
    private Collider[] _enemies;

    public DoTAreaOnReload(float tickTime, float totalDoTTime, float damage)
    {
        _tickTime = tickTime;
        _totalDoTTime = totalDoTTime;
        _damage = damage;
    }

    public override void ApplyOnReloadAreaEffect(float radius, Vector3 position)
    {
        _enemies = Physics.OverlapSphere(position, radius);
        Debug.Log("Overlap triggered");
        foreach (Collider enemy in _enemies)
        {
            if(enemy.gameObject.tag == "Enemy")
            {
                Debug.Log(enemy.gameObject.name);
                enemy.gameObject.GetComponentInParent<Enemy>().AddDoTStack(_damage, _totalDoTTime);
            }
        }
    }


    */

}
