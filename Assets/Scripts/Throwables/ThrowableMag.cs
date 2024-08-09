using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableMag : MonoBehaviour
{
    // NOTE UNFINISHED


    private Gun _originGun;
    private GameObject _reloadUpgradeObject;
    private OnReloadUpgrade _reloadUpgrade;

    private void Start()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.collider.gameObject;
        if (hitObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            float dotRemaining = 0f;
            dotRemaining = hitObject.GetComponentInParent<Enemy>().GetRemainingDoT();
           //if (dotRemaining != 0) _reloadUpgrade.AddToStacks(dotRemaining);
           // else
           // {
                //hitObject.GetComponentInParent<Enemy>().AddDoTStack(_reloadUpgrade.GetStacks(), 0.5f, 5f);
           //     _reloadUpgrade.ResetStacks();
           // }
        }

        Destroy(this.gameObject);
    }

    public void SetOriginGun(Gun gun)
    {
        _originGun = gun;
    }

    public void SetCurrentUpgrage(GameObject reloadUpgradeObject)
    {
        _reloadUpgradeObject = reloadUpgradeObject;
        _reloadUpgrade = reloadUpgradeObject.GetComponent<OnReloadUpgrade>();
    }


}
