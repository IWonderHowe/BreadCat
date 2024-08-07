using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    // To do: make idle state and combat state
    // in idle state: Check to see if the player is in visual range of the enemy (or check to see if got hit by player)
    // in combat state: Move to a good position, shoot, idle, repeat

    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private EnemyGun _gun;
    [SerializeField] private float _sightRange;
    [SerializeField] private LayerMask _raycastIgnoreLayer;
    [SerializeField] private GameObject _bulletOrigin;
    [SerializeField] private GameObject _target;

    [SerializeField] private bool _canMove = false;

    private Coroutine _currentCoroutine;

    private bool _hasPlayerLoS = false;

    private void Start()
    {
        _currentCoroutine = StartCoroutine(IdleState());



    }

    private void FixedUpdate()
    {
        //Debug.Log(CheckPlayerLoS());
        _hasPlayerLoS = CheckPlayerLoS();

        // look at the player if in LoS
        if (_hasPlayerLoS) SetEnemyFacing(transform.position - _target.transform.position);
    }


    private IEnumerator IdleState()
    {
        Debug.Log("Started idle state");
        while(!_hasPlayerLoS)
        {   
            yield return null;
        }
        StartCoroutine(AggroState());
    }

    private IEnumerator AggroState()
    {
        Debug.Log("Started aggro state");
        while (_hasPlayerLoS)
        {
            // if the enemy can move, do movement stuffs
            if (_canMove) ;

            SetEnemyFacing(transform.position - _target.transform.position);


            _gun.Shoot(_target);
            yield return new WaitForSeconds(_timeBetweenShots);
        }
        StartCoroutine(IdleState());
    }

    private void SetEnemyFacing(Vector3 direction)
    {
        Vector3 lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;

        transform.rotation = Quaternion.Euler(lookRotation);

    }

    private bool CheckPlayerLoS()
    {
        RaycastHit hit;

        // get direction of player in relation to the bullet origin
        Vector3 directionOfPlayer = _target.transform.position - _bulletOrigin.transform.position;

        
        // if the object hit by the raycast is the player, return true
        if(Physics.Raycast(_bulletOrigin.transform.position, directionOfPlayer, out hit, _sightRange, _raycastIgnoreLayer))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        };
        return false;

    }

    private void OnDrawGizmos()
    {
        Vector3 directionOfPlayer = _target.transform.position - _bulletOrigin.transform.position;
        if (Physics.Raycast(_bulletOrigin.transform.position, directionOfPlayer, out RaycastHit hit,  _sightRange, _raycastIgnoreLayer))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hit.point, _bulletOrigin.transform.position);

        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_target.transform.position, this.gameObject.transform.position);
    }
}
