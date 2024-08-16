using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class GrappleAbility : CharacterAbility
{
    public override string AbilityBaseMechanic { get { return _abilityBaseMechanic; } }
    private string _abilityBaseMechanic = "Point";

    private bool _hasUpgrade = false;
    private GameObject _upgrade;

    // variables to get/set info about player
    [SerializeField] private GameObject _playerObject;
    private PlayerMovement _playerMovement;
    private Camera _playerCam;

    // store info about the grapple point
    private Vector3[] _grappleInfo = new Vector3[2];
    private Vector3 _grapplePosition;
    private Vector3 _grappleNormal;


    // grappling hook variables
    [Header("Grappling Variables")]
    [SerializeField] private float _retractionSpeed = 5f;
    [SerializeField] private float _maxGrappleSpeed = 20f;
    [SerializeField] private float _grappleRange = 100f;
    [SerializeField] private LayerMask _grappleableLayers;
    [SerializeField] private float _grappleRetractDistanceBuffer = 1f;

    // grapple break variables
    [Header("Grapple Breaking")]
    [SerializeField] private float _minimumBreakAngle = 30f;
    [SerializeField] private float _breakAngleTolerance = 5f;
    [SerializeField] private float _camBreakAngle = 30f;
    [SerializeField] private float _breakAngle = 270;
    private Vector3 _camForwadVector;
    private float _grappleRotation = 0f;
    private Vector3 _previousPlayerToGrappleVector;

    [Header("Grapple break style. 0: Initial angle 1: Camera angle 2: Set angle")]
    [SerializeField] private bool[] _breakType = new bool[3];


    [SerializeField] private float _initialPointToPlayerAngle;
    [SerializeField] private float _currentGrappleAngle;

    private float _distanceToGrapplePoint = 0;

    private bool _isGrappling = false;
    public bool IsGrappling => _isGrappling;

    private bool _isRetracting = false;
    public bool IsRetracting => _isRetracting;

    private bool _isGrapplingEnemy = false;


    private float _retractionTime => (_distanceToGrapplePoint - _grappleRetractDistanceBuffer) / _retractionSpeed;



    private SpringJoint _grappleJoint;
    private LineRenderer _grappleRenderer;
    [SerializeField] private GameObject _grappleOriginPoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _playerMovement = _playerObject.GetComponent<PlayerMovement>();
        _grappleRenderer = GetComponent<LineRenderer>();
        _grappleRenderer.enabled = false;
        _playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // update the cameras forward vector
        _camForwadVector = Camera.main.transform.forward;

        // if the grapple joint exists, update the current grapple angle
        if (_grappleJoint != null)
        {
            _currentGrappleAngle = Vector3.Angle(Vector3.up, _grappleJoint.connectedAnchor - _playerObject.transform.position);
        }

        CheckBreakGrapple();


        if(IsGrappling && !IsRetracting && _distanceToGrapplePoint > (_grapplePosition - _playerObject.transform.position).magnitude)
        {
            _distanceToGrapplePoint = ((_grapplePosition - _playerObject.transform.position).magnitude * 0.95f);
            SetGrappleJointBounds();
        }
    }

    private void CheckBreakGrapple()
    {
        // if the player isnt grappling, dont worry about it
        if (!IsGrappling) return;

        // depending on which grapple break type, break the grapple
        // break grapple on initial grapple angle
        if (_breakType[0] == true)
        {
            if (_currentGrappleAngle > _initialPointToPlayerAngle)
            {
                StopMovementAbility();
                Debug.Log("Exit velocity: " + _playerObject.GetComponent<PlayerMovement>().CurrentSpeed);
            }
        }

        // break the grapple based on the angle between the grapple angle vs the players look direction
        else if (_breakType[1] == true)
        {
            Debug.Log(Vector3.Angle(_camForwadVector, _grapplePosition));
            // get angle between player cam forward and grapple point
            float grappleAngle = Vector3.Angle(_camForwadVector, _grappleJoint.connectedAnchor - _playerObject.transform.position);

            if (grappleAngle >= _camBreakAngle)
            {
                StopMovementAbility();
                Debug.Log("Exit velocity: " + _playerObject.GetComponent<PlayerMovement>().CurrentSpeed);
            }
        }

        // break based on a fixed angle from the start point
        else if (_breakType[2] == true)
        {
            // update the angle tracker
            _grappleRotation += Vector3.Angle(_previousPlayerToGrappleVector, _grapplePosition - _playerObject.transform.position);

            if(_grappleRotation >= _breakAngle)
            {
                StopMovementAbility();
                Debug.Log("Exit velocity: " + _playerObject.GetComponent<PlayerMovement>().CurrentSpeed);
            }

            _previousPlayerToGrappleVector = _grapplePosition - _playerObject.transform.position;
            Debug.Log(_grappleRotation);


        }

    }

    public override void UseAbility()
    {
        base.UseAbility();


        if (!_isGrappling && !_isRetracting)
        {
            RaycastHit hit;

            // raycast for the grapple, if nothing is hit, take ability off of cooldown and do nothing
            if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _grappleRange, _grappleableLayers))
            {
                _abilityOnCooldown = false;
                return;
            }

            // if there was a hit, store grapple info
            _grapplePosition = hit.point;
            _grappleNormal = hit.normal;

            // if the grapple hits an enemy, set the is grappling enemy to true
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                _isGrapplingEnemy = true;

                // if an upgrade for this ability is active, invoke it
                if (_hasUpgrade) { _upgrade.GetComponent<OnNonDamageAbilityUpgrade>().InvokeUpgrade(_playerObject, hit.collider.gameObject); }
            }

            // get the distance from the player to the grapple position
            _distanceToGrapplePoint = Vector3.Distance(_playerObject.transform.position, _grapplePosition);

            // set is grappling to true
            _isGrappling = true;
            _playerMovement.SetUsingMovementAbility(true);

            // connect the grapple
            StartGrapple(_grapplePosition);
        }

        else if(_isGrappling && !_isRetracting)
        {
            RetractGrapple();
        }

        else if(_isGrappling && _isRetracting)
        {
            StopMovementAbility();
        }
    }

    // get the grapple point via raycast
    private RaycastHit FindGrapplePoint()
    {
        // set variables for raycast info and grapple point. Using an array to store and return both position and normal of the grapple point found
        Vector3[] grappleInfo = new Vector3[2];
        RaycastHit hit;

        // if a raycast hits a valid object, set the grapple point to the transform of the hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _grappleRange, _grappleableLayers))
        {
            return hit;
        }

        return hit;
        // return found transform, or null if none was found
    }

    // connect the player to the grapple point by creating a spring joint joining player and grappled object
    private void StartGrapple(Vector3 grapplePoint)
    {
        
        // create the spring joing and connect it to the grapple point
        _grappleJoint = _playerObject.AddComponent<SpringJoint>();
        _grappleJoint.autoConfigureConnectedAnchor = false;
        _grappleJoint.connectedAnchor = grapplePoint;
        _grappleJoint.spring = 100f;
        _grappleJoint.tolerance = 0.05f;

        // set the initial angles of the grapple to define the grapple limits
        _previousPlayerToGrappleVector = grapplePoint - _playerObject.transform.position;
        _initialPointToPlayerAngle = Vector3.Angle(Vector3.up, grapplePoint - _playerObject.transform.position);


        // reset the angle tracker for the grapple
        _grappleRotation = 0f;

        SetGrappleJointBounds();
        SetRenderGrapple(true);
    }

    private void RetractGrapple()
    {
        StartCoroutine(GrappleRetraction());
        
    }

    // a coroutine that retracts the grapple distance
    private IEnumerator GrappleRetraction()
    {
        float retractStartTime = 0f;
        _isRetracting = true;

        while(retractStartTime <= _retractionTime && _isRetracting)
        {
            _distanceToGrapplePoint -= _retractionSpeed * Time.deltaTime;
            SetGrappleJointBounds();
            yield return null;
        }

        while(Vector3.Distance(transform.position, _grapplePosition) > _distanceToGrapplePoint)
        {
            yield return null;
        }

        StopMovementAbility();
    }

    private void SetGrappleJointBounds()
    {
        // set the max and mind distance of the grapple hook based on the players distance to the grapple point
        _grappleJoint.maxDistance = _distanceToGrapplePoint;
        _grappleJoint.minDistance = _distanceToGrapplePoint * 0.05f;
    }

    public override void StopMovementAbility()
    {
        if (!_isGrappling) return;
        _isRetracting = false;
        _isGrappling = false;
        _isGrapplingEnemy = false;
        SetRenderGrapple(false);
        _playerMovement.SetUsingMovementAbility(false);
        Destroy(_grappleJoint); 
    }

    private void SetRenderGrapple(bool isRendering)
    {
        if (!isRendering)
        {
            _grappleRenderer.enabled = false;
            return;
        }

        _grappleRenderer.enabled = true;
        _grappleRenderer.SetPosition(index: 0, _grappleOriginPoint.transform.position);
        _grappleRenderer.SetPosition(index: 1, _grapplePosition);

        StartCoroutine(UpdateGrappleRender());
    }

    private IEnumerator UpdateGrappleRender()
    {
        while (_isGrappling)
        {
            _grappleRenderer.SetPosition(index: 0, _grappleOriginPoint.transform.position);
            yield return null;
        }
    }

    public override void ApplyUpgrade(GameObject upgrade)
    {
        _hasUpgrade = true;
        _upgrade = upgrade;
    }
}
