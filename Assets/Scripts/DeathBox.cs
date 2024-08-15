using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    [SerializeField] private Vector3 _respawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.transform.position = _respawnPosition;
            other.gameObject.GetComponentInChildren<GrappleAbility>()?.StopMovementAbility();
        }
    }
}
