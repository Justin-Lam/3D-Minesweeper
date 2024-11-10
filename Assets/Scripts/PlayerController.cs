using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float acceleration;
    [SerializeField] float maxVelocity;

    Rigidbody rb;
    Vector3 moveDirection;


    void Start()
    {
        // Get rb
        rb = GetComponent<Rigidbody>();

        // Set max velocity
        rb.maxLinearVelocity = maxVelocity;
    }

    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }

	void FixedUpdate()
	{
        rb.AddRelativeForce(moveDirection * acceleration * Time.deltaTime);
	}
}
