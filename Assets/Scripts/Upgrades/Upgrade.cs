using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    public abstract void ApplyUpgrade(GameObject player);

}
