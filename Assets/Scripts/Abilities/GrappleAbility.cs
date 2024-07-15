using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GrappleAbility : CharacterAbility
{
    [SerializeField] private GameObject _playerObject;

    // grappling hook variables
    private Vector3[] _grappleInfo = new Vector3[2];
    private Vector3 _grapplePosition;
    private Vector3 _grappleNormal;
    [SerializeField] private float _retractionStrengh = 1f;
    [SerializeField] private float _maxGrappleSpeed = 20f;
    [SerializeField] private float _grappleRange = 100f;
    [SerializeField] private LayerMask _grappleableLayers;

    private bool _isGrappling = false;
    public bool IsGrappling => _isGrappling;

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
        if (_abilityOnCooldown) return;
        base.UseAbility();

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
        _isGrappling = true;

        StartGrapple(_grapplePosition);
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

    private void StartGrapple(Vector3 grapplePoint)
    {
        _grappleJoint = _playerObject.AddComponent<SpringJoint>();
        _grappleJoint.autoConfigureConnectedAnchor = false;
        _grappleJoint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(_playerObject.transform.position, grapplePoint);
        _grappleJoint.maxDistance = distanceFromPoint * 0.8f;
        _grappleJoint.minDistance = distanceFromPoint * 0.25f;

    }
}
