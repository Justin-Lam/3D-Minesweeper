using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	Transform lookAt;

	[Header("Panning")]
	[SerializeField] float panSensitivity;

	[Header("Zooming")]
	[SerializeField] float zoomSensitivity;
	[SerializeField] float minZoom;
	[SerializeField] float maxZoom;

	[Header("Rotating")]
	[SerializeField] float rotateSensitivity;
	float horizontalRotation = 0;
	float verticalRotation = 0;


	void Start()
	{
		lookAt = transform.parent;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Panning
		if (Input.GetMouseButton(0))	// left click
		{
			lookAt.Translate(Input.GetAxis("Mouse X") * panSensitivity, Input.GetAxis("Mouse Y") * panSensitivity, 0, Space.Self);
		}

		// Zooming
		transform.Translate(0, 0, Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity, Space.Self);

		// Rotating
		if (Input.GetMouseButton(1))	// right click
		{
			horizontalRotation += Input.GetAxis("Mouse X") * rotateSensitivity;
			verticalRotation -= Input.GetAxis("Mouse Y") * rotateSensitivity;
			lookAt.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
		}
	}

}
