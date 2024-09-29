using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Variables that define the enemy
    [SerializeField] private float _maxHealth;
    private EnemyCombat _enemyCombat;

    // space to store varaibles regarding enemy status
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isDead;

    // DoT variables
    private List<DoTStack> _dotStacks = new List<DoTStack>();
    public List<DoTStack> DoTStacks => _dotStacks;

    // UI properties
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _dotSlider;

    // space to store which room the enemy spawned into
    private RoomGenerator _roomResided;

    // a game event for when the enemy dies
    [SerializeField] private GameObjectEvent _onDeath;

    // a game event to ping this enemy
    [SerializeField] private GameObjectEvent _levelManagerResponse;

    // grounded properties
    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    [SerializeField] private float _groundCheckOffset = 0.1f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private float _groundedFudgeTime = 0.25f;
    [SerializeField] private LayerMask _groundMask = 1 << 0;
    [SerializeField] private bool _isGrounded;
    private Vector3 _groundCheckStart => transform.position + transform.up * _groundCheckOffset;
    private float _lastGroundedTime;

    // set space for the navmesh agent
    private NavMeshAgent _agent;
    public NavMeshAgent Agent => _agent;
    public int NavMeshAreaMask => _agent.areaMask;

    // space for rigidbody
    private Rigidbody _enemyRB;

    // Start is called before the first frame update 
    void Start()
    {
        // get necessary components from the enemy
        _enemyCombat = GetComponent<EnemyCombat>();
        _enemyRB = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();

        // set the enemy to be alive and at full health
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: refactor grounding for enemies
        _isGrounded = IsGrounded;

        // TODO: refactor how DoT stacks are applied to enemies (maybe apart from the enemy object?)
        float dotDamageLeft = 0;
        foreach (DoTStack i in _dotStacks)
        {
            dotDamageLeft += i.DoTDamageRemaining;
        }

        // check grounded
        IsGrounded = CheckGrounded();

        // set is kinematic and gravity based on if grounded
        /*
        if (IsGrounded && !_enemyRB.isKinematic)
        {
            _enemyRB.isKinematic = true;
            _enemyRB.useGravity = false;

            // TODO: refactor which state the enemy goes into after landing
            _enemyCombat.SetEnemyState("ChaseState");
        }*/

        // update the enemy health UI
        UpdateHealthBar();

    }

    // Check to see if the player is on the ground by raycasting downwards
    private bool CheckGrounded()
    {
        // shoot a ray downwards and store hit information
        bool hit = Physics.Raycast(_groundCheckStart, -transform.up, out RaycastHit hitInfo, _groundCheckDistance, _groundMask);
        GroundNormal = Vector3.up;

        // if there is no ray hit, return the player isnt grounded
        if (!hit) return false;

        // if the raycast hits the ground, return the player is grounded (and the most recent time they were grounded for coyote time)
        if (hit)
        {
            _lastGroundedTime = Time.timeSinceLevelLoad;
            return true;
        }

        return false;
    }

    public void MoveTo(Vector3 destination)
    {
        
        // unhalt the movement
        if (_agent.isStopped) _agent.isStopped = false;

        // set the nav mesh agents destination
        _agent.destination = destination;

    }

    public void StopMovement()
    {
        // stop enemy movement
        _agent.isStopped = true;
    }

    public void TakeDamage(float damage)
    {
        // take damage if the enemy isn't dead. Set the enemy to dead if the current health drops to 0
        if (!_isDead) _currentHealth -= damage;
        if (_currentHealth <= 0) OnDeath();
    }

    private void UpdateHealthBar()
    {
        // TODO: refactor how DoT shows up on enemy health bar
        float remainingDoT = GetRemainingDoT();
        if (remainingDoT == 0) _dotSlider.gameObject.SetActive(false);
        else
        {
            _dotSlider.gameObject.SetActive(true);
            _dotSlider.value = GetRemainingDoT() / _maxHealth;
        }

        // update the health bar to show current health in relation to the max health
        _healthSlider.value = _currentHealth / _maxHealth;
        
    }

    
    // TODO: refactor how the DoT stacks end and are accounted for
    public void EndDoTStack(DoTStack stackToEnd)
    {
        // remove a finished stack of DoT from the enemy. remove it from the DoT affected enemies list if it has no more DoT stacks left
        _dotStacks.Remove(stackToEnd);
        if (_dotStacks.Count == 0) DoTStack.RemoveEnemyFromDoTList(this.gameObject);
    }

    // TODO: refactor where this method is from, conglomerate the DoT within one area rather than on each enemy
    public float GetRemainingDoT()
    {
        float dotRemaining = 0f;

        foreach(DoTStack dotStack in _dotStacks)
        {
            dotRemaining += dotStack.DoTDamageRemaining;
        }

        if (dotRemaining > _currentHealth) dotRemaining = _currentHealth;

        return dotRemaining;
    }


    private void OnDeath()
    {
        // remove the enemy from the list of enemies in the room
        _roomResided?.RemoveFromEnemyList(this.gameObject);
        
        // set the enemy to being dead
        _isDead = true;

        // call the death event
        _onDeath.Invoke(this.gameObject);

        // destroy this enemy
        Destroy(this.gameObject);
    }

    // invoke the level manager poplulate event
    public void RespondToLevelManager()
    {
        _levelManagerResponse.Invoke(this.gameObject);
    }

    public void SetRoom(RoomGenerator roomResided)
    {
        // set the enemies room based on given room generator
        _roomResided = roomResided;
    }

    public void AddForceToEnemy(Vector3 force)
    {
        
    }

    public void PauseEnemyAgent(float time)
    {
        StartCoroutine(PauseNavMesh(time));
    }

    // pause the nav mesh for a set time
    // TODO: refactor how the enemy swaps between using navmesh and physics
    private IEnumerator PauseNavMesh(float time)
    {
        // an end time and disable navmesh
        float endTime = Time.timeSinceLevelLoad + time;

        // disable the enemy navmesh agent, and set the enemy to use physics instead
        _agent.enabled = false;
        _enemyRB.isKinematic = false;
        _enemyRB.useGravity = true;

        // keep doing this until the end time
        while (Time.timeSinceLevelLoad <= endTime)
        {
            yield return null;
        }

        // reset the enemy to use its navmesh again instead of physics
        _enemyRB.isKinematic = true;
        _enemyRB.useGravity = false;
        _agent.enabled = true;
    }

}
