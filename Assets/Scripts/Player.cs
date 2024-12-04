using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	[Header("Animations")]
	public Animator anim;
	float time = 0.0f;

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
	bool wasGrounded = true;        // whether IsGrounded() was true last frame or not
	bool usedFastFall = false;
	float groundedDistFromGround;   // the max distance the player can be from the ground in order to be grounded
	float groundedDistFromGroundPadding = 0.1f; // (10%)

	[Header("Flagging")]
	[SerializeField] GameObject flag;

	[Header("Singleton Pattern")]
	private static Player instance;
	public static Player Instance { get { return instance; } }
	void Singleton_SetInstance()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}


	void Awake()
	{
		Singleton_SetInstance();
	}

	void Start()
	{
		// Get rb
		rb = GetComponent<Rigidbody>();

		// Get gameplay camera panner
		gameplayCameraPanner = GameObject.Find("Panner").transform;

		// Calculate groundedDistFromGround
		float colliderRadius = GetComponent<SphereCollider>().radius;
		groundedDistFromGround = colliderRadius * (1 + groundedDistFromGroundPadding);

		// Get Animator
		anim = GetComponent<Animator>();
	}

	void Update()
	{
		// Idle look around animation
		if (time > 10) 
		{
			anim.SetBool("isLooking", true);
			time = 0;
		}
		time += Time.deltaTime;

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
		// Check for jump end
		if (JustLanded())
		{
			//print("jump done");
			anim.SetBool("isJumping", false);
			anim.SetBool("isIdle", true);
		}

		// Handle fast falling
		// asked ChatGPT for help on how to integrate my old fast falling method with the new jump buffer system: "what's the best way to make it so that the velocity reduction from fast falling only happens once per jump"
		if (!Input.GetButton("Jump") && !usedFastFall && rb.velocity.y > 0)
		{
			usedFastFall = true;
			rb.AddForce(Vector3.down * rb.velocity.y * letGoOfJumpVelocityDecreaseRatio, ForceMode.VelocityChange);
		}

		// Check for eat and flag
		if (Input.GetButtonDown("Eat"))
		{
			Eat();
		}
		if (Input.GetButtonDown("Flag"))
		{
			Flag();
		}

		// Set wasGrounded (this must come at the end of Update() so the next Update() call can use it)
		wasGrounded = IsGrounded();
	}

	void FixedUpdate()
	{
		// Walking animations
		if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
		{
			anim.CrossFade("Walking", 0.2f);
		}
		if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
		{
			anim.SetBool("isWalking", true);
			anim.SetBool("isIdle", false);
		}
		else
		{
			anim.SetBool("isWalking", false);
			anim.SetBool("isIdle", true);
		}
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
		return !wasGrounded && IsGrounded();    // wasn't grounded last frame but is now
	}

	void Jump()
	{
		// Animations
		anim.SetBool("isJumping", true);
		anim.SetBool("isIdle", false);

		// Terminate jump buffer counter
		jumpBufferCounter = 0;

		// Set y velocity to 0 so in case we're falling our jump isn't weakened by the current downwards velocity
		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

		// Jump
		rb.AddForce(Vector3.up * jumpStrength, ForceMode.VelocityChange);

		// Set usedFastFall
		usedFastFall = false;
	}

	void Eat()
	{
		// Can only eat if grounded
		RaycastHit hit;
		if (IsGroundedOnSomething(out hit))
		{
			// Can only eat if also grounded on a block
			if (hit.collider.gameObject.CompareTag("Block"))
			{
				// Get the block and call its OnEat()
				hit.collider.gameObject.GetComponent<Block>().OnEat();
			}
		}
	}
	void Flag()
	{
		// Check that the player is grounded and set hit
		RaycastHit hit;
		if (!IsGroundedOnSomething(out hit))
		{
			return;
		}

		// Flag
		if (hit.collider.gameObject.CompareTag("Block"))    // standing on a block
		{
			// Jump
			Jump();

			// Get the block's transform
			Transform block = hit.collider.transform;

			// Spawn flag centered on top of the block
			Instantiate(flag, new Vector3(block.position.x, block.position.y + block.localScale.y / 2, block.position.z), Quaternion.identity);
		}

		// Unflag
		else if (hit.collider.gameObject.CompareTag("Flag"))    // standing on a flag
		{
			// Destroy the flag
			Destroy(hit.collider.gameObject);
		}
	}
	bool IsGroundedOnSomething(out RaycastHit hit)
	{
		return Physics.Raycast(transform.position, Vector3.down, out hit, groundedDistFromGround);
	}

	public void OnAffectedByExplosion()
	{
		rb.drag = 0;                                // so player falls as fast as everything else
		rb.constraints = RigidbodyConstraints.None; // so player rotates like everything else
		enabled = false;                            // so player loses control of the player character
	}
}