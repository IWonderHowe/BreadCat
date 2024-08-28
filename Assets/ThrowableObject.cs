using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class ThrowableObject : MonoBehaviour
{
    abstract public ThrowableType Type { get; }

}

public enum ThrowableType
{
    ExplosionShrapnel,
    ExlposionRadius,
    Impact
}
