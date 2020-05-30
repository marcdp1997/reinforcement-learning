using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCentralized : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 90;
    public bool isGrounded = true;

    private Vector3 initialPos;
    private Rigidbody rBody;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        initialPos = transform.position;
    }

    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("ground"))
            isGrounded = true;
    }

    public void Reset()
    {
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
        transform.position = initialPos;
    }

    public Rigidbody GetRigidBody()
    {
        return rBody;
    }
}
