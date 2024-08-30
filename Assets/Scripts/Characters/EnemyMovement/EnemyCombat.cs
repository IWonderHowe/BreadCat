using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    // get relevant layers for sightlines
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _visionLayers;

    // get gun and relevant properties
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private EnemyGun _gun;
    [SerializeField] private float _sightRange;
    [SerializeField] private GameObject _bulletOrigin;

    // set a space aside to store the current target and whether LoS is established
    private GameObject _target;
    private Vector3 _targetLastSeenPosition;
    private bool _hasTargetLoS = false;
    
    // enemy movement properties
    [SerializeField] private bool _canMove = false;
    private float _navIgnoreDistance = 5f;
    private int _navMask;
    [SerializeField] private float _strafeDistance = 1f;


    // a place to store the other scripts on this enemy
    private Enemy _thisEnemy;

    private Coroutine _currentCoroutine;


    // debugging
    [SerializeField] private string _currentState;

    private void Awake()
    {
        // begin enemy in their idle state
        _currentCoroutine = StartCoroutine(IdleState());
        _thisEnemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        // see if the enemy can currently see the player
        _hasTargetLoS = CheckTargetLoS();

        // look at the player if in LoS
        if (_hasTargetLoS) SetEnemyFacing(transform.position - _target.transform.position);
    }

    public void GetTargetObject(GameObject player)
    {
        //if (_target == null) return;
        _target = player.gameObject;
    }

    // TODO: refactor the enemy states to use an enum rather than strings
    public void SetEnemyState(string state)
    {
        switch (state)
        {
            case "FlungState":
                StopAllCoroutines();
                StartCoroutine(FlungState());
                return;

            case "IdleState":
                StopAllCoroutines();
                StartCoroutine(IdleState());
                return;

            case "AggroState":
                StopAllCoroutines();
                StartCoroutine(AggroState());
                return;

            case "ChaseState":
                StopAllCoroutines();
                StartCoroutine(ChaseState(_target.transform.position));
                return;

            default:
                Debug.Log("InvalidState");
                return;

        }
    }


    private IEnumerator FlungState()
    {
        yield return new WaitForEndOfFrame();

        _currentState = "FlungState";

        while (!_thisEnemy.IsGrounded)
        {
            Debug.Log("FlungStateFrame");
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        if (CheckTargetLoS())
        {
            StartCoroutine(AggroState()); 
        }

        
        else StartCoroutine(ChaseState(_targetLastSeenPosition));

    }


    private IEnumerator IdleState()
    {
        // set the current state to be idle
        _currentState = "IdleState";

        // while the enemy doesn't have a target, do nothing
        while (_target == null) yield return null;

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
        // set the current state to be aggro
        _currentState = "AggroState";

        // while the enemy has LoS on the target
        while (_hasTargetLoS)
        {
            // if the enemy can move, do movement stuffs
            if (_canMove);

            // shoot at the target then wait for the proper time between shots
            _gun.Shoot(_target.GetComponentInChildren<PlayerCombat>().gameObject);
            yield return new WaitForSeconds(_timeBetweenShots);

        }
        
        // upon breaking LoS, try to chase the player
        StartCoroutine(ChaseState(_target.transform.position));
    }

    private IEnumerator ChaseState(Vector3 playerLastSeen) 
    {
        // set the current state to be chase
        _currentState = "ChaseState";

        // if the enemy navmesh agent isnt active and enabled, do nothing
        while (!_thisEnemy.Agent.isActiveAndEnabled) yield return new WaitForEndOfFrame();

        // move the player towards the players last seen location
        NavMeshHit destinationHit;

        // if the players last seen position is close to a navmesh area point, go to that point to try to find the player
        if (NavMesh.SamplePosition(playerLastSeen, out destinationHit, _navIgnoreDistance, NavMesh.AllAreas))
        {
            Debug.Log("theres a destination");
            _thisEnemy.MoveTo(destinationHit.position);
        }

        // if the enemy doesnt have LoS keep moving toward last seen point
        while (!_hasTargetLoS)
        {
            yield return new WaitForFixedUpdate();
        }


        // upon seeing the player, stop going to players last seen position but not immediately
        yield return new WaitForSeconds(0.2f);
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
        // if the enemy doesnt have a target, there can't be LoS
        if (_target == null) return false;

        // store a spot to store hit data
        RaycastHit hit;

        // get direction of player in relation to the bullet origin
        Vector3 directionOfPlayer = _target.transform.position - _bulletOrigin.transform.position;
        
        // if the object hit by the raycast is the player, return true
        if(Physics.SphereCast(_bulletOrigin.transform.position, 1f, directionOfPlayer, out hit, _sightRange, _visionLayers))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _targetLastSeenPosition = hit.collider.gameObject.transform.position;
                return true;
            }
        };
        return false;

    }

}

public enum EnemyState
{

}
