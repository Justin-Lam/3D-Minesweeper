using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Horizontal Movement")]
	[SerializeField] float acceleration;
	[SerializeField] float maxVelocity;

	[Header("Jumping")]
	[SerializeField] float jumpStrength;

	Rigidbody rb;
	Vector3 moveDirection;


	void Start()
	{
		// Get rb
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		// Get move direction
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

		// Check for jump
		if (Input.GetButtonDown("Jump"))
		{
			// Jump
			rb.AddForce(Vector3.up * jumpStrength, ForceMode.VelocityChange);
		}
	}

	void FixedUpdate()
	{
		// Apply movement force
		rb.AddRelativeForce(moveDirection * acceleration * Time.deltaTime, ForceMode.Acceleration);

		// Limit movement velocity
		Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		if (horizontalVelocity.magnitude > maxVelocity)
		{
			// Calculate the counter-force needed to maintain max velocity
			float amountOverMaxVelocity = horizontalVelocity.magnitude - maxVelocity;
			Vector3 counterForce = -horizontalVelocity.normalized * amountOverMaxVelocity;

			// Apply counter-force
			rb.AddRelativeForce(counterForce, ForceMode.VelocityChange);
		}
	}
}
