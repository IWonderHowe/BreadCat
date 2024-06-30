using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.Locked;


    private Gun _gun;
    private CharacterMovement _movement;

    private void Awake()
    {
        _gun = GetComponent<Gun>();
        _movement = GetComponent<CharacterMovement>();
        Cursor.lockState = _cursorMode;
    }

    private void Update()
    {
        _movement.SetCharacterFacing(Camera.main.transform.rotation.eulerAngles.y);
    }

    public void OnMove(InputValue value)
    {
        
    }

    public void OnFire(InputValue value)
    {
        _gun.ShootRayCast();
        Debug.Log(value.isPressed);
    }


}
