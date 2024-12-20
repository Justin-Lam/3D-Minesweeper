using System;
using UnityEngine;

public class Player : MonoBehaviour
{
	[Header("Horizontal Movement")]
	[SerializeField] float acceleration;
	[SerializeField] float maxVelocity;
	[SerializeField] float airDrag;
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
	SphereCollider sphereCollider;
	float groundedDistFromGround;   // the max distance the player can be from the ground in order to be grounded
	float groundedDistFromGroundPadding = 0.3f; // 30%
	Vector3 jumpRaycastOrigin { get { return transform.TransformPoint(sphereCollider.center); } }

	[Header("Flagging")]
	[SerializeField] GameObject flag;
	[SerializeField] Transform eatRaycastOrigin;
	[SerializeField] Transform flagRaycastOrigin;
	public static event Action OnFlagPlaced;

	[Header("Animations")]
	Animator anim;
	float time = 0.0f;

	[Header("Particles")]
	public ParticleSystem eatEffect;

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
		rb = GetComponent<Rigidbody>();
		gameplayCameraPanner = GameObject.Find("Panner").transform;
		sphereCollider = GetComponent<SphereCollider>();
		groundedDistFromGround = sphereCollider.radius/2 * (1 + groundedDistFromGroundPadding);
		anim = GetComponent<Animator>();
	}

	void Update()
	{
		// Get move direction
		moveDirection = (gameplayCameraPanner.forward * Input.GetAxis("Vertical") + gameplayCameraPanner.right * Input.GetAxis("Horizontal")).normalized;

		// Handle rotational movement
		if (moveDirection != Vector3.zero)
		{
			transform.forward = Vector3.Slerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);	// rotate towards move direction
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
			anim.SetBool("isJumping", false);
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
			// Animations
			stopAnimations();
			anim.SetTrigger("isEating");
		}
		if (Input.GetButtonDown("Flag"))
		{
			OnFlag();
			// Animations
			stopAnimations();
			anim.SetTrigger("isFlagging");
		}

		// Handle walking animations
		if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
		{
			anim.CrossFade("Walking", 0.2f);
		}
		if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
		{
			stopAnimations();
			anim.SetBool("isWalking", true);
		}
		else
		{
			anim.SetBool("isWalking", false);
			anim.SetBool("isIdle", true);
		}

		// Handle idle animation
		if (time > 10)
		{
			anim.SetTrigger("isLooking");
			time = 0;
		}
		time += Time.deltaTime;

		// Set wasGrounded (this must come at the end of Update() so the next Update() call can use it)
		wasGrounded = IsGrounded();
	}

	void FixedUpdate()
	{
		// Apply acceleration
		rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);

		// Limit velocity
		// asked ChatGPT for help on how to do set the rigidbody's horizontal velocity to have a magnitude of maxVelocity
		Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		if (horizontalVelocity.magnitude > maxVelocity)
		{
			horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
			rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
		}

		// Apply air drag
		// asked ChatGPT for help on how to do this: "how can i apply drag to my player until they're no longer moving when they stop giving movement inputs?"
		if (moveDirection == Vector3.zero && horizontalVelocity.magnitude > 0 && !IsGrounded())
		{
			Vector3 direction = -horizontalVelocity.normalized;
			// Ensure we don't reverse direction due to drag
			if (airDrag < horizontalVelocity.magnitude)
			{
				rb.AddForce(direction * airDrag, ForceMode.Acceleration);
			}
			else
			{
				rb.velocity = new Vector3(0, rb.velocity.y, 0);
			}
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
		return Physics.Raycast(jumpRaycastOrigin, Vector3.down, groundedDistFromGround);
	}
	bool JustLanded()
	{
		return !wasGrounded && IsGrounded();    // wasn't grounded last frame but is now
	}

	void Jump()
	{
		// Animations
		stopAnimations();
		anim.SetBool("isJumping", true);

		// Terminate jump buffer counter
		jumpBufferCounter = 0;

		// Set y velocity to 0 so in case we're falling our jump isn't weakened by the current downwards velocity
		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

		// Jump
		rb.AddForce(Vector3.up * jumpStrength, ForceMode.VelocityChange);

		// Set usedFastFall
		usedFastFall = false;

		// Animations
		anim.SetBool("isJumping", true);
		anim.SetBool("isIdle", false);
	}

	void Eat()
	{
		// Can only eat if grounded
		RaycastHit hit;
		if (IsGroundedOnSomething(eatRaycastOrigin.position, out hit))
		{
			// Can only eat if also grounded on a block
			if (hit.collider.gameObject.CompareTag("Block"))
			{
				// Get the block and call its OnEat()
				hit.collider.gameObject.GetComponent<Block>().OnEat();
				// Particles
				eatEffect.Play();
			}
		}
	}

	public void OnFlag()
    {
		OnFlagPlaced?.Invoke();
	}
	public void HandleOnFlag()
	{
		// Check that the player is grounded and set hit
		RaycastHit hit;
		if (!IsGroundedOnSomething(flagRaycastOrigin.position, out hit))
		{
			return;
		}

		// Flag
		if (hit.collider.gameObject.CompareTag("Block"))    // standing on a block
		{

			// Get the block's transform
			Transform block = hit.collider.transform;
			Instantiate(flag, new Vector3(block.position.x, block.position.y + block.localScale.y / 2, block.position.z), Quaternion.identity);
		}

		// Unflag
		else if (hit.collider.gameObject.CompareTag("Flag"))    // standing on a flag
		{
			Destroy(hit.collider.gameObject);
		}
	}
	bool IsGroundedOnSomething(Vector3 raycastOrigin, out RaycastHit hit)
	{
		return Physics.Raycast(raycastOrigin, Vector3.down, out hit, groundedDistFromGround);
	}

	public void OnAffectedByExplosion()
	{
		rb.drag = 0;                                // so player falls as fast as everything else
		rb.constraints = RigidbodyConstraints.None; // so player rotates like everything else
		enabled = false;                            // so player loses control of the player character
	}

	void stopAnimations()
	{
		anim.SetBool("isIdle", false);
		anim.ResetTrigger("isLooking");
		anim.SetBool("isWalking", false);
	}
}