using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GrappleAbility : CharacterAbility
{
    // grappling hook variables
    private Transform _grapplePoint;
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

        _grapplePoint = FindGrapplePoint();

        Debug.Log(_grapplePoint.position);

    }

    // get the grapple point via raycast
    private Transform FindGrapplePoint()
    {
        // set variables for raycast info and grapple point
        Transform grapplePoint = null;
        RaycastHit hit;

        // if a raycast hits a valid object, set the grapple point to the transform of the hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _grappleRange, _grappleableLayers))
        {
            grapplePoint = hit.transform;
        }

        // return found transform, or null if none was found
        return grapplePoint;
    }
}
