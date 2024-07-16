using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Note: Base architecture of this class is reliant on scripts from VFS tutors Scott and Quinn
public class CharacterMovement : MonoBehaviour
{ 
    // make some information pbulic
    public Vector3 MoveInput { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Vector3 Velocity { get => _rigidbody.velocity; private set => _rigidbody.velocity = value; }
    public bool IsFudgeGrounded => Time.timeSinceLevelLoad < _lastGroundedTime + _groundedFudgeTime;
    public bool IsGrounded { get; private set; }

    [SerializeField] private float _speedLimit = 20f;

    // ground movement
    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _sprintSpeedMultiplier = 2f;

    // jump variables
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _airControl = 0.1f;
            
    // ground checks
    [SerializeField] private float _groundCheckOffset = 0.1f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private float _groundedFudgeTime = 0.25f;
    [SerializeField] private LayerMask _groundMask = 1 << 0;
    private Vector3 _groundCheckStart => transform.position + transform.up * _groundCheckOffset;
    private float _lastGroundedTime;

    // store information about movement
    private bool _isSprinting = false;
    private float _speed = 0f;

    // necessary classes for applying force
    private Rigidbody _rigidbody;

    private bool _isUsingMovementAbility;
    public bool IsUsingMovementAbility => _isUsingMovementAbility;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _speed = _baseSpeed;
    }

    private void FixedUpdate()
    {
        // Check to see if the character is grounded
        IsGrounded = CheckGrounded();

        // take the movement input and calculate what is forward in relation to input and player
        Vector3 input = MoveInput;
        Vector3 right = Vector3.Cross(transform.up, input);
        Vector3 forward = Vector3.Cross(right, GroundNormal);

        // find a target velocity on calculated forward vector
        Vector3 targetVelocity = forward * (_speed);


        // have a different movement system if using movement ability or not on the ground
        /*if(_isUsingMovementAbility || !IsGrounded)
        {
            targetVelocity = forward * _speedLimit;
        }*/
        
        // Find the difference in velocity between the current and target velocity (flattened)
        Vector3 velocityDiff = targetVelocity - Velocity;
        velocityDiff.y = 0f;

        // Calculate the acceleration and apply to character
        Vector3 acceleration = velocityDiff * _acceleration;

        // just add acceleration to player direclty in relation to their input (rather than current velocity) if in air or using movement ability 
        if(!IsGrounded || IsUsingMovementAbility)
        {
            if(targetVelocity.magnitude < Velocity.magnitude)
            {
                acceleration = forward * (_speed);
            }
        }

        // add gravity's acceleration then apply the acceleration to the player
        acceleration += GroundNormal * _gravity;
        _rigidbody.AddForce(acceleration);
        

        // make the character face where the player is aiming
        SetCharacterFacing(Camera.main.transform.rotation.eulerAngles);



    }

    // Set the player to face a certain direction (most often the direction they are aiming
    public void SetCharacterFacing(Vector3 direction)
    {
        direction.x = 0;
        direction.z = 0;
        transform.rotation = Quaternion.Euler(direction);
    }

    // Take the movement input and adjusut it so that it is normalized and flattened
    public void SetMoveInput(Vector3 input)
    {
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 flattened = new Vector3(input.x, 0f, input.z);
        flattened = flattened.normalized * input.magnitude;

        MoveInput = flattened;
    }

    // Check to see if the player is on the ground by raycasting downwards
    private bool CheckGrounded()
    {
        // shoot a ray downwards and store hit information
        bool hit = Physics.Raycast(_groundCheckStart, -transform.up, out RaycastHit hitInfo, _groundCheckDistance, _groundMask);
        GroundNormal = Vector3.up;

        // if there is no ray hit, return the player isnt grounded
        if(!hit) return false;

        // if the raycast hits the ground, return the player is grounded (and the most recent time they were grounded for coyote time)
        if (hit)
        {
            _lastGroundedTime = Time.timeSinceLevelLoad;
            return true;
        }

        return false;
    }

    // Jump if the player is able to
    public void Jump()
    {
        if (!IsFudgeGrounded) return;
        float jumpVelocity = Mathf.Sqrt(2f * -_gravity * _jumpHeight);
        Velocity = new Vector3(Velocity.x, jumpVelocity, Velocity.z);
    }

    // Apply sprint speed multiplier if the player is sprinting
    public void SetSprint(bool isSprinting)
    {
        if (isSprinting) { _speed = _baseSpeed * _sprintSpeedMultiplier; }
        else _speed = _baseSpeed;
    }

    private void OnDrawGizmos()
    {

    }
    public void SetUsingMovementAbility(bool isUsingAbility)
    {
        _isUsingMovementAbility = isUsingAbility;
    }
}
