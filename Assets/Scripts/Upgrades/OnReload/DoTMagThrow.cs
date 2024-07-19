using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class DoTMagThrow : OnReloadUpgrade
{
    [SerializeField] private GameObject _magObject;

    public override void ThrowableMagReloadEffect(Vector3 throwOrigin, float throwForce, Gun gun)
    {
        GameObject mag = Instantiate(_magObject, throwOrigin, Quaternion.identity);
        mag.GetComponent<ThrowableMag>().SetOriginGun(gun);
        mag.GetComponent<Rigidbody>().AddForce(throwForce * Camera.main.transform.forward);

    }



}
