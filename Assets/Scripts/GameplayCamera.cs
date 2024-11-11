using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	Transform lookAt;
	Transform verticalPivot;

	[Header("Panning")]
	[SerializeField] float panSensitivity;

	[Header("Zooming")]
	[SerializeField] float zoomSensitivity;
	[SerializeField] float minZoom;
	[SerializeField] float maxZoom;

	[Header("Rotating")]
	[SerializeField] float rotateSensitivity;



	void Start()
	{
		verticalPivot = transform.parent;
		lookAt = verticalPivot.parent;
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Panning
		if (Input.GetMouseButton(0))	// left click
		{
			lookAt.Translate(-Input.GetAxis("Mouse X") * panSensitivity, 0, -Input.GetAxis("Mouse Y") * panSensitivity, Space.Self);
		}

		// Zooming
		transform.Translate(0, 0, Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity, Space.Self);

		// Rotating
		if (Input.GetMouseButton(1))	// right click
		{
			lookAt.Rotate(0, Input.GetAxis("Mouse X") * rotateSensitivity, 0, Space.Self);
			verticalPivot.Rotate(-Input.GetAxis("Mouse Y") * rotateSensitivity, 0, 0, Space.Self);


/*			horizontalRotation += Input.GetAxis("Mouse X") * rotateSensitivity;
			verticalRotation -= Input.GetAxis("Mouse Y") * rotateSensitivity;
			lookAt.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);*/
		}
	}

}
