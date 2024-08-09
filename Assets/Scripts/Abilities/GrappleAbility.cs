using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEditor.Build;
using UnityEngine;

public class GrappleAbility : CharacterAbility
{
    public override string AbilityBaseMechanic { get { return _abilityBaseMechanic; } }
    private string _abilityBaseMechanic = "Point";

    // variables to get/set info about player
    [SerializeField] private GameObject _playerObject;
    private PlayerMovement _playerMovement;

    // grappling hook variables
    private Vector3[] _grappleInfo = new Vector3[2];
    private Vector3 _grapplePosition;
    private Vector3 _grappleNormal;
    [SerializeField] private float _retractionSpeed = 5f;
    [SerializeField] private float _maxGrappleSpeed = 20f;
    [SerializeField] private float _grappleRange = 100f;
    [SerializeField] private LayerMask _grappleableLayers;
    [SerializeField] private float _grappleRetractDistanceBuffer = 1f;
    [SerializeField] private float _minimumBreakAngle = 30f;
    [SerializeField] private float _breakAngleTolerance = 5f;

    private float _distanceToGrapplePoint = 0;
    [SerializeField] private float _initialGrappleAngle;
    [SerializeField] private float _currentGrappleAngle;
    private bool _isGrappling = false;
    public bool IsGrappling => _isGrappling;

    private bool _isRetracting = false;
    public bool IsRetracting => _isRetracting;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (_grappleJoint != null)
        {
            //Debug.Log("grappled");
            _currentGrappleAngle = Vector3.Angle(Vector3.up, _grappleJoint.connectedAnchor - _playerObject.transform.position);
        }
        //Debug.Log("Initial angle: " + _initialGrappleAngle + "             Current angle: " + _currentGrappleAngle);
        // stop grappling if the player goes beyond their able grapple angle 
        if (IsGrappling && _currentGrappleAngle > _initialGrappleAngle)
        {
            StopMovementAbility();
            Debug.Log("Exit velocity: " + _playerObject.GetComponent<PlayerMovement>().CurrentSpeed);
        }

        if(IsGrappling && !IsRetracting && _distanceToGrapplePoint > (_grapplePosition - _playerObject.transform.position).magnitude)
        {
            _distanceToGrapplePoint = ((_grapplePosition - _playerObject.transform.position).magnitude * 0.95f);
            SetGrappleJointBounds();
        }
    }

    public override void UseAbility()
    {
        base.UseAbility();


        if(!_isGrappling && !_isRetracting)
        {
            // get the grapple info and set the grapple normal and position to the point that was 
            _grappleInfo = FindGrapplePoint();
            
            // if no grapple point is found, exit the method and dont put the ability on cooldown
            if (_grappleInfo == null)
            {
                _abilityOnCooldown = false;
                return;
            }

            // if a grapple point is found, set the info and set the player to be grappling
            _grapplePosition = _grappleInfo[0];
            _grappleNormal = _grappleInfo[1];
            _distanceToGrapplePoint = Vector3.Distance(_playerObject.transform.position, _grapplePosition);

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
    private Vector3[] FindGrapplePoint()
    {
        // set variables for raycast info and grapple point. Using an array to store and return both position and normal of the grapple point found
        Vector3[] grappleInfo = new Vector3[2];
        RaycastHit hit;

        // if a raycast hits a valid object, set the grapple point to the transform of the hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _grappleRange, _grappleableLayers))
        {
            grappleInfo[0] = hit.point;
            grappleInfo[1] = hit.normal;
            return grappleInfo;
        }

        // return found transform, or null if none was found
        return null;
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

        // set the initial angle of the grapple to define the grapple limits
        _initialGrappleAngle = Vector3.Angle(Vector3.up, grapplePoint - _playerObject.transform.position);

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
        throw new System.NotImplementedException();
    }
}
