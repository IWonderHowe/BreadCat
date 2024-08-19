using System.Collections;
using System.Collections.Generic;
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
        foreach(Collider hit in _objectsHit)
        {
            GameObject hitObject = hit.gameObject.transform.parent.gameObject;

            if(hit.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemiesHit.Contains(hitObject))
            { 
                hit.gameObject.GetComponentInParent<Enemy>().TakeDamage(damage);

                // add the enemy gameobject to the list
                enemiesHit.Add(hitObject);
                Debug.Log("enemy hit");
            }

            // damage enemy with base damage


            // apply ability upgrade if necessary
            //if (hit.gameObject.CompareTag("Enemy") && _hasOnEnemyHitUpgrade) _grenadeHitEnemyUpgrade.ApplyOnAbilityHit(hit.gameObject.GetComponentInParent<Enemy>(), damage);
        }


        if(_hasUpgrade) _upgrade.GetComponent<OnDamageAbilityUpgrade>().InvokeUpgrade(_objectsHit);

        // Show gizmo and wait for a second before destroying the grenade
        _grenadeExploded = true;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

    public void SetUpgrade(GameObject upgrade)
    {
        _hasUpgrade = true;
        _upgrade = upgrade;
    }

    // a method to allow the start of the grenade coroutine from another script
    public void LightFuse(float explosionRadius, float timeToExplosion, float damage, LayerMask damageableLayers, GameObject parentObject)
    {
        StartCoroutine(LiveGrenade(explosionRadius, timeToExplosion, damage, damageableLayers));
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
