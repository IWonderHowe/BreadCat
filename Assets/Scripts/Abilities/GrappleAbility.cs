using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GrappleAbility : CharacterAbility
{
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

        // get the grapple info and set the grapple normal and position to the point that was found
        // exit the method if no grapple point is found
        _grappleInfo = FindGrapplePoint();
        if (_grappleInfo == null) return;

        _grapplePosition = _grappleInfo[0];
        _grappleNormal = _grappleInfo[1];

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
}
