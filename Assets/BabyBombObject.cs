using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BabyBombObject : ThrowableObject
{
    public override ThrowableType Type => throw new System.NotImplementedException();
    private ThrowableType _type = ThrowableType.ExplosionShrapnel;

    // shrapnel properties
    [Header("Shrapnel Properties")]
    [SerializeField] private GameObject _bonePrefab;
    [SerializeField] private float _boneInitialSpeed;
    [SerializeField] private float _boneDamage;
    [SerializeField] private float _boneGravity;
    [SerializeField] private int _bonesOnExplosion;
    [SerializeField] private float _explosionMaxAngle;
    [SerializeField] private float _explosionForce;

    // baby properties
    [Header("Baby Properties")]
    [SerializeField] private float _babyDamage;


    // gizmo variables
    Vector3 _explosionNormal;
    Vector3 _explosionPoint;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponentInParent<Enemy>().TakeDamage(_babyDamage);
        }

        _explosionNormal = collision.GetContact(0).normal;
        _explosionPoint = collision.GetContact(0).point;

        Debug.Log(Vector3.Angle(Vector3.up, _explosionNormal));

        ExplodeIntoBones(_bonesOnExplosion, collision.GetContact(0).normal);
        Destroy(this.gameObject);

    }

    private void ExplodeIntoBones(int numBones, Vector3 explosionNormal)
    {
        // get angles for each bone
        Vector3[] boneOutwardAngles = new Vector3[numBones];
        for (int i = 0; i < numBones; i++)
        {
            Vector3 boneAngle = explosionNormal;

            boneAngle += new Vector3(Random.Range(-_explosionMaxAngle, _explosionMaxAngle), Random.Range(-_explosionMaxAngle, _explosionMaxAngle), Random.Range(-_explosionMaxAngle, _explosionMaxAngle));

            // set the i-th bone angle to be this bone angle
            boneOutwardAngles[i] = boneAngle.normalized;
        }

        //instantiate bones at baby position and shoot them outwards
        for (int i = 0; i < numBones; i++)
        {
            // instantiate the bone
            GameObject boneInstantiated = Instantiate(_bonePrefab);

            // set the transform apart from the baby bomb
            boneInstantiated.transform.position = transform.position;

            // shoot out the bone
            boneInstantiated.GetComponent<Rigidbody>().AddForce(boneOutwardAngles[i] * _explosionForce, ForceMode.Impulse);
        }

       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_explosionNormal != null) Gizmos.DrawLine(_explosionPoint, _explosionNormal + _explosionPoint);
    }
}
