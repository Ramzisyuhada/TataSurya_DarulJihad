using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPresentPhysic : MonoBehaviour
{
    public Rigidbody rb;
    public Transform target;
    void Start()
    {

    }

    private void Awake()
    {

    }
    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        physic(target);




    }

    private void physic(Transform target)
    {
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        //rotation
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffrenceInDegree = angleInDegree * rotationAxis;

        rb.angularVelocity = (rotationDiffrenceInDegree * Mathf.Rad2Deg / Time.fixedDeltaTime);
    }
   
}
