using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.Locked;

    private Gun _gun;
    private void Awake()
    {
        _gun = GetComponent<Gun>();
        Cursor.lockState = _cursorMode;
    }

    public void OnMove(InputValue value)
    {
        
    }

    public void OnFire(InputValue value)
    {
        _gun.ShootRayCast();
    }


}
