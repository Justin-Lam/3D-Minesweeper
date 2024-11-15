using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Horizontal Movement")]
	[SerializeField] float acceleration;
	[SerializeField] float maxVelocity;
	Rigidbody rb;
	Vector3 moveDirection;

	[Header("Rotational Movement")]
	[SerializeField] float rotateSpeed;
	Transform gameplayCameraPanner;

	[Header("Jumping")]
	[SerializeField] float jumpStrength;
	[SerializeField] float jumpBufferDuration;
	[SerializeField] float letGoOfJumpVelocityDecreaseRatio;
	[SerializeField] float extraGravityWhenFallingAcceleration;
	float jumpBufferCounter = 0;
	bool wasGrounded = true;		// whether IsGrounded() was true last frame or not
	bool usedFastFall = false;
	float groundedDistFromGround;   // the max distance the player can be from the ground in order to be grounded
	float groundedDistFromGroundPadding = 0.1f; // (10%)
	private RaycastHit blockHit;            // for use in checking for block collision
	private Block blockScript;


	void Start()
	{
		// Get rb
		rb = GetComponent<Rigidbody>();

		// Get gameplay camera panner
		gameplayCameraPanner = GameObject.Find("Panner").transform;

		// Calculate groundedDistFromGround
		float colliderRadius = GetComponent<SphereCollider>().radius;
		groundedDistFromGround = colliderRadius * (1 + groundedDistFromGroundPadding);
	}

	void Update()
	{
		// Get move direction
		moveDirection = (gameplayCameraPanner.forward * Input.GetAxis("Vertical") + gameplayCameraPanner.right * Input.GetAxis("Horizontal")).normalized;

		// Rotate towards move direction
		if (moveDirection != Vector3.zero)
		{
			transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
		}

		// Handle jump buffer
		if (Input.GetButtonDown("Jump"))
		{
			jumpBufferCounter = jumpBufferDuration;
		}
		if (jumpBufferCounter > 0)
		{
			jumpBufferCounter -= Time.deltaTime;
		}

		// Check for jump
		if (jumpBufferCounter > 0 && IsGrounded())
		{
			Jump();
		}
		else if (Input.GetButton("Jump") && JustLanded())
		{
			Jump();
		}

		// Handle fast falling
		// asked ChatGPT for help on how to integrate my old fast falling method with the new jump buffer system: "what's the best way to make it so that the velocity reduction from fast falling only happens once per jump"
		if (!Input.GetButton("Jump") && !usedFastFall && rb.velocity.y > 0)
		{
			usedFastFall = true;
			rb.AddForce(Vector3.down * rb.velocity.y * letGoOfJumpVelocityDecreaseRatio, ForceMode.VelocityChange);
		}

		// Set wasGrounded (this must come at the end of Update() so the next Update() call can use it)
		wasGrounded = IsGrounded();

		if (Input.GetKeyDown(KeyCode.E))
		{
			OnEat();
		}
	}

	void FixedUpdate()
	{
		// Apply movement force
		rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);

		// Limit movement velocity
		// asked ChatGPT for help on how to do this: "how can i apply a vector in the opposite direction the player is moving in to ensure they don't go past max velocity"
		Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		if (horizontalVelocity.magnitude > maxVelocity)
		{
			// Calculate the counter-force needed to maintain max velocity
			float amountOverMaxVelocity = horizontalVelocity.magnitude - maxVelocity;
			Vector3 counterForce = -horizontalVelocity.normalized * amountOverMaxVelocity;

			// Apply counter-force
			rb.AddForce(counterForce, ForceMode.VelocityChange);
		}

		// Apply extra gravity when falling
		if (rb.velocity.y < 0)
		{
			rb.AddForce(Vector3.down * extraGravityWhenFallingAcceleration, ForceMode.Acceleration);
		}
	}

	bool IsGrounded()
	{
		// got this from https://discussions.unity.com/t/using-raycast-to-determine-if-player-is-grounded/85134/2
		return Physics.Raycast(transform.position, Vector3.down, groundedDistFromGround);
	}
	bool JustLanded()
	{
		return !wasGrounded && IsGrounded();	// wasn't grounded last frame but is now
	}

	void Jump()
	{
		// Terminate jump buffer counter
		jumpBufferCounter = 0;

		// Set y velocity to 0 so in case we're falling our jump isn't weakened by the current downwards velocity
		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

		// Jump
		rb.AddForce(Vector3.up * jumpStrength, ForceMode.VelocityChange);

		// Set usedFastFall
		usedFastFall = false;
	}

	bool IsOnBlock()
	{
		if (Physics.Raycast(transform.position, Vector3.down, out blockHit, groundedDistFromGround))
		{
			if (blockHit.collider != null && blockHit.collider.tag == "Block")
			{
				return true;
			}
		}

		return false;
	}

	void OnEat()
    {
		if (IsOnBlock())
        {
			GameObject block = blockHit.transform.gameObject;
			blockScript = block.GetComponent<Block>();
			blockScript.OnEat();
		}
    }
}
