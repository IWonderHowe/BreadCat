using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.Locked;


    [SerializeField] private GameObject _equippedGun;
    private Gun _currentGun;
    private CharacterMovement _movement;

    private Vector2 _moveInput;

    private void Awake()
    {

        _currentGun = _equippedGun.GetComponent<Gun>();
        _movement = GetComponent<CharacterMovement>();
        Cursor.lockState = _cursorMode;
    }
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        _currentGun.SetIsShooting(value.isPressed);
        Debug.Log(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        _movement.SetSprint(value.isPressed);
    }

    public void OnJump(InputValue value)
    {
        _movement.Jump();
    }

    public void OnReload(InputValue value)
    {
        _currentGun.TriggerReload();
    }

    private void Update()
    {
        Vector3 up = Vector3.up;
        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(right, up);
        Vector3 moveInput = forward * _moveInput.y + right * _moveInput.x;


        _movement.SetMoveInput(moveInput);
    }

}
