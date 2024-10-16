using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // store the cursor lock mode
    [SerializeField] private CursorLockMode _cursorMode = CursorLockMode.Locked;

    // TODO: refactor player to only have one gun
    // Space for the gun gameobjects that are equipped to the player, as well as specific references to the gun scripts attached
    [SerializeField] private GameObject _gunObject1;
    [SerializeField] private GameObject _gunObject2;
    private Gun _gun1;
    private Gun _gun2;

    // keep track of which gun is currently active
    [SerializeField] private Gun _currentGun;
    public Gun CurrentGun => _currentGun;

    // get the players movement script and establish a space to store the move input (for normalization)
    private PlayerMovement _movement;
    private Vector2 _moveInput;

    // space for the abilities to be equipped/stored
    [SerializeField] GameObject _ability1Object;
    [SerializeField] GameObject _ability2Object;
    public GameObject Ability1Object => _ability1Object;
    public GameObject Ability2Object => _ability2Object;
    
    private CharacterAbility _ability1;
    private CharacterAbility _ability2;

    // get the player input
    private static PlayerInput _playerInput;

    // a ping to let other gameobjects know of the players existence
    [SerializeField] private GameObjectEvent _playerPing;
    private Vector3 _initialPosition;

    // debug object
    [SerializeField] private DebugMenu _debug;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // when a new scene is loaded, ping the player out to all objects
        if(this != null) StartCoroutine(PingRoutine());
    }

    private void Awake()
    {
        // ping the player on waking
        StartCoroutine(PingRoutine());

        // Get necessary components from gameobjects
        _gun1 = _gunObject1.GetComponent<Gun>();
        _gun2 = _gunObject2.GetComponent<Gun>();
        _ability1 = _ability1Object.GetComponent<CharacterAbility>();
        _ability2 = _ability2Object.GetComponent<CharacterAbility>();
        _movement = GetComponent<PlayerMovement>();

        // set the players initial position
        _initialPosition = transform.position;

        // Set the current gun to the first gun
        _currentGun = _gun1.GetComponent<Gun>();

        // lock the cursor to the game screen
        Cursor.lockState = _cursorMode;
        
        // get the player input
        _playerInput  = GetComponent<PlayerInput>();

        // set the main camera to be the player FPS camera
        _playerInput.camera = Camera.main;
    }
    
    // TODO: change the player reset to be a part of the on death event
    // reset the player back to the entry of the room
    public void ResetPlayer()
    {
        _ability1.StopMovementAbility();
        _ability2.StopMovementAbility();

        transform.position = _initialPosition;
       
        _currentGun.TriggerReload();
    }

    private IEnumerator PingRoutine()
    {
        // wait for a second to ping the player to the other objects in the scene
        // used to get around load orders
        yield return new WaitForSeconds(1f);
        _playerPing.Invoke(gameObject);
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
        // stop the movement ability if jump is pressed
        if (_movement.IsUsingMovementAbility)
        {
            if (_ability1.IsMovementAbility)
            {
                _ability1.StopMovementAbility();
            }    
            if(_ability2.IsMovementAbility)
            {
                _ability2.StopMovementAbility();
            }
            return;
        }

        // jump
        _movement.Jump();
    }

    // reload if the player presses the reload button
    public void OnReload(InputValue value)
    {
        // reload the gun
        _currentGun.TriggerReload();

        // set the reload to be a manual reload
        _currentGun.SetIsManualReloading(true);
    }

    public void OnOpenDebug(InputValue value)
    {
        _debug.gameObject.SetActive(!_debug.gameObject.activeInHierarchy);

        if (_debug.gameObject.activeInHierarchy) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.Locked;

    }

    // attempt to use the primary ability if button pressed
    public void OnAbility1(InputValue value)
    {
        _ability1.UseAbility(value.isPressed);
    }

    // attempt to use the secondary ability if button pressed
    public void OnAbility2(InputValue value)
    {
        if (value.isPressed == false) return;
        _ability2.UseAbility();
    }

    // Swap the current player weapon to the unequipped weapon when mouse is scrolled
    // TODO: get rid of weapon swaps as a part of the single weapon current functionality
    public void OnSwapWeapon(InputValue value)
    {
        //dont do this currently
        return;
        if (!value.isPressed) return;
        if (_currentGun == _gun1) _currentGun = _gun2;
        else _currentGun = _gun1;
    }

    // toggle the mouse lock state for debug purposes/buttons
    public void OnToggleLockMouse(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.Confined;
    }

    // set whether the player can input anything
    public void SetPlayerInputActive(bool isActive)
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.enabled = isActive;
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
