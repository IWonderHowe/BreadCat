using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // create space for an array to store all objects hit by explosion
    private Collider[] _objectsHit;

    // variable to adjust gizmo visibility
    private bool _grenadeExploded = false;

    //private OnAbilityHitEnemyUpgrade _grenadeHitEnemyUpgrade;
    private bool _hasUpgrade;
    private GameObject _upgrade;

    private GameObject _abilityObject;
    private float _explosionForce;


    private void Awake()
    {
    }

    private IEnumerator LiveGrenade(float explosionRadius, float timeToExplosion, float damage, LayerMask interactableLayers)
    {
        // set initial grenade variables when thrown
        float timeSinceFuse = 0f;

        
        
        // Wait for the desired time until the grenade explodes
        while(timeSinceFuse < timeToExplosion)
        {
            timeSinceFuse += Time.deltaTime;
            yield return null;
        }

        // See which objects are within the explosion radius and damage objects that are tagged as an enemy
        _objectsHit = Physics.OverlapSphere(this.transform.position, explosionRadius, interactableLayers);
        List<GameObject> enemiesHit = new List<GameObject>();
        foreach (Collider hit in _objectsHit)
        {
            GameObject hitObject = hit.gameObject.transform.parent.gameObject;

            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemiesHit.Contains(hitObject))
            {
                hit.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage);

                // add the enemy gameobject to the list
                enemiesHit.Add(hitObject);
                // allow physics to affect the enemy
                hitObject.GetComponent<Enemy>().PauseEnemyAgent(3f);

                //damage enemy
                hitObject.GetComponent<Enemy>().TakeDamage(damage);
                hitObject.GetComponent<Rigidbody>().AddForce((hit.transform.position - transform.position).normalized * _explosionForce, ForceMode.Impulse);
                hitObject.GetComponent<EnemyCombat>().SetEnemyState("FlungState");

                Debug.Log("enemy hit");
            }

            // set the player object to not be the player container but rather the player
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hitObject = hit.gameObject.GetComponentInChildren<PlayerCombat>().gameObject;
                hitObject.GetComponent<PlayerMovement>().PopUsingMovementAbility();


                hitObject.GetComponent<Rigidbody>().AddForce((hit.transform.position - transform.position).normalized * _explosionForce, ForceMode.Impulse);
            }
            
            // send the hit object outwards from grenade
            else hitObject.GetComponent<Rigidbody>().AddForce((hit.transform.position - transform.position).normalized * _explosionForce, ForceMode.Impulse);

            // apply upgrade to enemies hit
            if (_hasUpgrade) _upgrade.GetComponent<OnDamageAbilityUpgrade>().InvokeUpgrade(enemiesHit.ToArray());

            // Show gizmo and wait for a second before destroying the grenade
            _grenadeExploded = true;
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
    }

    public void SetUpgrade(GameObject upgrade)
    {
        _hasUpgrade = true;
        _upgrade = upgrade;
    }

    // a method to allow the start of the grenade coroutine from another script
    public void LightFuse(float explosionRadius, float timeToExplosion, float damage, float explosionForce, LayerMask damageableLayers, GameObject parentObject)
    {
        StartCoroutine(LiveGrenade(explosionRadius, timeToExplosion, damage, damageableLayers));
        _explosionForce = explosionForce;
        _abilityObject = parentObject;
    }

    

    // Show where explosion is in red
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_grenadeExploded) Gizmos.DrawSphere(this.transform.position, 3f);
    }

    // a way to apply abilities to the enemy if applicable
    /*
    public void AddAbilityOnEnemyHit(OnAbilityHitEnemyUpgrade onEnemyHitUpgrade)
    {
        _grenadeHitEnemyUpgrade = onEnemyHitUpgrade;
        _hasOnEnemyHitUpgrade = true;
    }*/
}
