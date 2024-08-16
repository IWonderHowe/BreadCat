using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    // To do: make idle state and combat state
    // in idle state: Check to see if the player is in visual range of the enemy (or check to see if got hit by player)
    // in combat state: Move to a good position, shoot, idle, repeat

    // get relevant layers for sightlines
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _visionLayers;

    // get gun and relevant properties
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private EnemyGun _gun;
    [SerializeField] private float _sightRange;
    [SerializeField] private GameObject _bulletOrigin;

    // set a space aside to store the current target and whether LoS is established
    [SerializeField] private GameObject _target;
    private bool _hasTargetLoS = false;
    
    // enemy movement properties
    [SerializeField] private bool _canMove = false;
    private float _navIgnoreDistance = 5f;
    [SerializeField] private int _navMask;


    // a place to store the other scripts on this enemy
    private Enemy _thisEnemy;

    private Coroutine _currentCoroutine;


    private void Start()
    {
        // begin enemy in their idle state
        _currentCoroutine = StartCoroutine(IdleState());
        _thisEnemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        //Debug.Log(CheckPlayerLoS());
        // see if the enemy can currently see the player
        _hasTargetLoS = CheckTargetLoS();

        // look at the player if in LoS
        if (_hasTargetLoS) SetEnemyFacing(transform.position - _target.transform.position);
    }


    private IEnumerator IdleState()
    {
        Debug.Log("Started idle state");
        // do nothing while the enemy does not have LoS with target
        while(!_hasTargetLoS)
        {   
            yield return null;
        }

        // when LoS is established, enter the aggro state
        StartCoroutine(AggroState());
    }

    private IEnumerator AggroState()
    {
        Debug.Log("Started aggro state");

        // while the enemy has LoS on the target
        while (_hasTargetLoS)
        {
            // if the enemy can move, do movement stuffs
            if (_canMove);

            // shoot at the target then wait for the proper time between shots
            _gun.Shoot(_target);
            yield return new WaitForSeconds(_timeBetweenShots);
        }

        
        // upon breaking LoS, return to the idle state
        StartCoroutine(ChaseState(_target.transform.position));
    }

    private IEnumerator ChaseState(Vector3 playerLastSeen) 
    {


        // move the player towards the players last seen location
        NavMeshHit destinationHit;

        if (NavMesh.SamplePosition(playerLastSeen, out destinationHit, _navIgnoreDistance, NavMesh.AllAreas))
        {
            Debug.Log("theres a destination");
            _thisEnemy.MoveTo(destinationHit.position);
        }

        // if the enemy doesnt have LoS keep chasing
        while (!_hasTargetLoS)
        {
            yield return new WaitForFixedUpdate();
        }


        // stop going to players last seen position but not immediately
        yield return new WaitForSeconds(0.5f);
        _thisEnemy.StopMovement();


        // when LoS is established go aggro
        StartCoroutine(AggroState());
    }

    private void SetEnemyFacing(Vector3 direction)
    {
        // Set the look rotation to be towards the target, then flatten the look direction
        Vector3 lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;

        // set the current rotation to be that of the look rotation
        transform.rotation = Quaternion.Euler(lookRotation);

    }

    private bool CheckTargetLoS()
    {
        // store a spot to store hit data
        RaycastHit hit;

        // get direction of player in relation to the bullet origin
        Vector3 directionOfPlayer = _target.transform.position - _bulletOrigin.transform.position;

        
        // if the object hit by the raycast is the player, return true
        if(Physics.Raycast(_bulletOrigin.transform.position, directionOfPlayer, out hit, _sightRange, _visionLayers))
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
        // show a blue line for enemy LoS
        Vector3 directionOfPlayer = _target.transform.position - _bulletOrigin.transform.position;
        if (Physics.Raycast(_bulletOrigin.transform.position, directionOfPlayer, out RaycastHit hit,  _sightRange, _visionLayers))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(hit.point, _bulletOrigin.transform.position);

        }
    }
}
