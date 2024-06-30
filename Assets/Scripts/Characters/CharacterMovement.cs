using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;

    public Vector3 MoveInput { get; private set; }
    public Vector3 GroundNormal { get; private set; } = Vector3.up;
    public Vector3 Velocity { get => _rigidbody.velocity; protected set => _rigidbody.velocity = value; }


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
}
