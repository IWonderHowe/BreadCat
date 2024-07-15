using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEngine;

public class GrappleAbility : CharacterAbility
{
    [SerializeField] private GameObject _playerObject;

    // grappling hook variables
    private Vector3[] _grappleInfo = new Vector3[2];
    private Vector3 _grapplePosition;
    private Vector3 _grappleNormal;
    [SerializeField] private float _retractionSpeed = 5f;
    [SerializeField] private float _maxGrappleSpeed = 20f;
    [SerializeField] private float _grappleRange = 100f;
    [SerializeField] private LayerMask _grappleableLayers;
    [SerializeField] private float _grappleRetractDistanceBuffer = 1f;

    private bool _isGrappling = false;
    public bool IsGrappling => _isGrappling;

    private bool _isRetracting = false;
    public bool IsRetracting => _isRetracting;

    private float _distanceToGrapplePoint = 0;
    private float _retractionTime => (_distanceToGrapplePoint - _grappleRetractDistanceBuffer) / _retractionSpeed;

    private SpringJoint _grappleJoint;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

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
                Debug.Log("No grapple point found");
                return;
            }

            // if a grapple point is found, set the info and set the player to be grappling
            _grapplePosition = _grappleInfo[0];
            _grappleNormal = _grappleInfo[1];
            _distanceToGrapplePoint = Vector3.Distance(_playerObject.transform.position, _grapplePosition);

            _isGrappling = true;

            // connect the grapple
            StartGrapple(_grapplePosition);
        }

        else if(_isGrappling && !_isRetracting)
        {
            RetractGrapple();
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

        SetGrappleJointBounds();
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
    }

    private void SetGrappleJointBounds()
    {
        // set the max and mind distance of the grapple hook based on the players distance to the grapple point
        _grappleJoint.maxDistance = _distanceToGrapplePoint * 0.8f;
        _grappleJoint.minDistance = _distanceToGrapplePoint * 0.25f;
    }
}
