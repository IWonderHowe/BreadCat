using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Vector3 _groundCheckStart => transform.position + transform.up * _groundCheckOffset;
    private float _lastGroundedTime;

    public Vector3 MoveInput { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Vector3 Velocity { get => _rigidbody.velocity; private set => _rigidbody.velocity = value; }
    public bool IsFudgeGrounded => Time.timeSinceLevelLoad < _lastGroundedTime + _groundedFudgeTime;
    public bool IsGrounded { get; private set; }


    // ground movement
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _acceleration = 10f;

    // jump variables
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _airControl = 0.1f;
            
    // ground checks
    [SerializeField] private float _groundCheckOffset = 0.1f;
    [SerializeField] private float _groundCheckDistance = 0.4f;
    [SerializeField] private float _groundedFudgeTime = 0.25f;
    [SerializeField] private LayerMask _groundMask = 1 << 0;

    


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Check to see if the character is grounded
        IsGrounded = CheckGrounded();


        //SetCharacterFacing(Camera.main.transform.rotation.eulerAngles.y);

        Vector3 input = MoveInput;
        Vector3 right = Vector3.Cross(transform.up, input);
        Vector3 forward = Vector3.Cross(right, GroundNormal);

        Vector3 targetVelocity = forward * (_speed);

        Vector3 velocityDiff = targetVelocity - Velocity;
        velocityDiff.y = 0f;

        Vector3 acceleration = velocityDiff * _acceleration;

        acceleration += GroundNormal * _gravity;


        _rigidbody.AddForce(acceleration);




    }

    public void SetCharacterFacing(float direction)
    {
        transform.rotation = Quaternion.Euler(0, direction, 0);
    }

    public void SetMoveInput(Vector3 input)
    {
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 flattened = new Vector3(input.x, 0f, input.z);
        flattened = flattened.normalized * input.magnitude;

        MoveInput = flattened;
    }

    private bool CheckGrounded()
    {
        bool hit = Physics.Raycast(_groundCheckStart, -transform.up, out RaycastHit hitInfo, _groundCheckDistance, _groundMask);

        GroundNormal = Vector3.up;

        if(!hit) return false;

        if (hit)
        {
            _lastGroundedTime = Time.timeSinceLevelLoad;
            return true;
        }

        return false;
    }

    public void Jump()
    {
        if (!IsFudgeGrounded) return;
        float jumpVelocity = Mathf.Sqrt(2f * -_gravity * _jumpHeight);
        Velocity = new Vector3(Velocity.x, jumpVelocity, Velocity.z);
    }

    private void OnDrawGizmos()
    {

    }
}
