using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // set space for all relevant parent colliders and current collider
    private Collider _projectileCollider;
    private GameObject _parentObject;
    private List<Collider> _collidersToIgnore = new List<Collider>();

    private void Awake()
    {
        // set the projectile collider to be this collider
        _projectileCollider = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        // if triggered by player, hit the player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit player");
        }

        // destroy projectile when it hits another collider
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit player");
        }
        Destroy(this.gameObject);
    }


    public void SetParentObject(GameObject parent) 
    {
        // set the parent of the projectile
        _parentObject = parent;

        // ignore all colliders in the parent for this projectile
        foreach (Collider parentCollider in GetComponentsInChildren<Collider>())
        {
            _collidersToIgnore.Add(parentCollider);
            Physics.IgnoreCollision(_projectileCollider, parentCollider);
        }
    }
}
