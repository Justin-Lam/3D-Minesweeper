using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Horizontal Movement")]
	[SerializeField] float acceleration;
	[SerializeField] float maxVelocity;

	[Header("Jumping")]
	[SerializeField] float jumpStrength;
	[SerializeField] float jumpBufferDuration;
	[SerializeField] float letGoOfJumpVelocityDecreaseRatio;
	[SerializeField] float extraGravityWhenFallingAcceleration;

	Rigidbody rb;
	Vector3 moveDirection;
	float jumpBufferCounter = 0;
	bool fastFalling = true;
	bool usedFastFall = false;
	float groundedDistFromGround;   // the max distance the player can be from the ground in order to be grounded
	float groundedDistFromGroundPadding = 0.1f;	// (10%)


	void Start()
	{
		// Get rb
		rb = GetComponent<Rigidbody>();

		// Calculate groundedDistFromGround
		float colliderRadius = GetComponent<SphereCollider>().radius;
		groundedDistFromGround = colliderRadius * (1 + groundedDistFromGroundPadding);
	}

	void Update()
	{
		// Get move direction
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

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

		// Handle fast falling
		fastFalling = !Input.GetButton("Jump");
		if (fastFalling && rb.velocity.y > 0 && !usedFastFall)
		{
			usedFastFall = true;
			rb.AddForce(Vector3.down * rb.velocity.y * letGoOfJumpVelocityDecreaseRatio, ForceMode.VelocityChange);
		}
	}

	void FixedUpdate()
	{
		// Apply movement force
		rb.AddRelativeForce(moveDirection * acceleration * Time.deltaTime, ForceMode.Acceleration);

		// Limit movement velocity
		// asked ChatGPT for help on how to do this: "how can i apply a vector in the opposite direction the player is moving in to ensure they don't go past max velocity"
		Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		if (horizontalVelocity.magnitude > maxVelocity)
		{
			// Calculate the counter-force needed to maintain max velocity
			float amountOverMaxVelocity = horizontalVelocity.magnitude - maxVelocity;
			Vector3 counterForce = -horizontalVelocity.normalized * amountOverMaxVelocity;

			// Apply counter-force
			rb.AddRelativeForce(counterForce, ForceMode.VelocityChange);
		}

		// Apply extra gravity
		if (rb.velocity.y < 0)
		{
			rb.AddForce(Vector3.down * extraGravityWhenFallingAcceleration * Time.deltaTime, ForceMode.Acceleration);
		}
	}

	bool IsGrounded()
	{
		// got this from https://discussions.unity.com/t/using-raycast-to-determine-if-player-is-grounded/85134/2
		return Physics.Raycast(transform.position, Vector3.down, groundedDistFromGround);
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
}
