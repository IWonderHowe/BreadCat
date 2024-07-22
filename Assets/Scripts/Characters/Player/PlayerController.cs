using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.Locked;


    // Space for the gun gameobjects that are equipped to the player, as well as specific references to the gun scripts attached
    [SerializeField] private GameObject _gunObject1;
    [SerializeField] private GameObject _gunObject2;
    private Gun _gun1;
    private Gun _gun2;

    // keep track of which gun is currently active
    [SerializeField] private Gun _currentGun;
    public Gun CurrentGun => _currentGun;

    // get the players movement script and establish a space to store the move input (for normalization)
    private CharacterMovement _movement;
    private Vector2 _moveInput;

    // space for the abilities to be equipped/stored
    [SerializeField] GameObject _ability1Object;
    [SerializeField] GameObject _ability2Object;
    
    private CharacterAbility _ability1;
    private CharacterAbility _ability2;



    private void Awake()
    {

        // Get necessary components from gameobjects
        _gun1 = _gunObject1.GetComponent<Gun>();
        _gun2 = _gunObject2.GetComponent<Gun>();
        _ability1 = _ability1Object.GetComponent<CharacterAbility>();
        _ability2 = _ability2Object.GetComponent<CharacterAbility>();
        _movement = GetComponent<CharacterMovement>();
        
        // Set the current gun to the first gun
        _currentGun = _gun1.GetComponent<Gun>();

        // lock the cursor to the game screen
        Cursor.lockState = _cursorMode;
    }

    // adjust moveinput variable based in input
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    // fire the current gun when fire button pressed
    public void OnFire(InputValue value)
    {
        _currentGun.SetIsShooting(value.isPressed);
    }

    // allow sprinting when sprint button is held
    public void OnSprint(InputValue value)
    {
        _movement.SetSprint(value.isPressed);
    }

    // try to jump if the player presses the jump button
    public void OnJump(InputValue value)
    {
        _movement.Jump();
        if (_ability1.IsMovementAbility) _ability1.CancelMovementAbility();
        if(_ability2.IsMovementAbility) _ability2.CancelMovementAbility();
    }

    // reload if the player presses the reload button
    public void OnReload(InputValue value)
    {
        // reload the gun
        _currentGun.TriggerReload();

        // set the reload to be a manual reload
        _currentGun.SetIsManualReloading(true);

        // reset chaos stacks if applicable
        if (ChaosStack.Stacks != 0) ChaosStack.ResetStacks();
    }

    // attempt to use the primary ability if button pressed
    public void OnAbility1(InputValue value)
    {
        _ability1.UseAbility();
    }

    // attempt to use the secondary ability if button pressed
    public void OnAbility2(InputValue value)
    {
        if (value.isPressed == false) return;
        _ability2.UseAbility();
    }

    // Swap the current player weapon to the unequipped weapon when mouse is scrolled
    public void OnSwapWeapon(InputValue value)
    {
        if (!value.isPressed) return;
        if (_currentGun == _gun1) _currentGun = _gun2;
        else _currentGun = _gun1;
    }

    public void OnToggleLockMouse(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        // Get vectors in relation to the player and their aim
        Vector3 up = Vector3.up;
        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(right, up);

        // Calculate move input based on relative vectors and apply this input to the movement script
        Vector3 moveInput = forward * _moveInput.y + right * _moveInput.x;
        _movement.SetMoveInput(moveInput);
    }

}
