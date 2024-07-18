using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTMagThrow : OnReloadUpgrade
{
    private float _dotStored = 0f;

    public override void ThrowableMagReloadEffect(Vector3 throwOrigin, float throwForce, GameObject mag)
    {
        GameObject activeMag = mag;
    }

    public void StoreDoT(float DoTToStore)
    {
        _dotStored += DoTToStore;
    }



}
