using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Collider _projectileCollider;
    private GameObject _parentObject;

    private List<Collider> _collidersToIgnore = new List<Collider>();

    private void Awake()
    {
        _projectileCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("hit player");
        }
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
        _parentObject = parent;
        foreach (Collider parentCollider in GetComponentsInChildren<Collider>())
        {
            _collidersToIgnore.Add(parentCollider);
            Physics.IgnoreCollision(_projectileCollider, parentCollider);
        }
    }
}
