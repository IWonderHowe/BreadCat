using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableMag : MonoBehaviour
{
    private Collider _magCollider;
    private GameObject _originGun;

    private void Start()
    {
        _magCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            
        }
    }

    public void SetOriginGun(GameObject gun)
    {
        _originGun = gun;
    }


}
