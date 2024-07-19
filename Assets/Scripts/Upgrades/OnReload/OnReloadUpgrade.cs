using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnReloadUpgrade : MonoBehaviour
{
    protected float _stacks = 0;

    public virtual void ApplyOnReloadAreaEffect(float radius, Vector3 position)
    {

    }

    public virtual void ThrowableMagReloadEffect(Vector3 throwOrigin, float throwForce, Gun gun)
    {

    }

    public virtual void AddToStacks(float numberToAdd)
    {
        _stacks += numberToAdd;
    }

    public virtual float GetStacks() 
    {
        return _stacks;
    }

    public virtual void ResetStacks()
    {
        _stacks = 0;
    }
}
