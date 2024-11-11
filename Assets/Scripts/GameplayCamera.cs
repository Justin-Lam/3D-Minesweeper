using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	[Header("Children")]
	// this double parent setup is necessary so vertical panning doesn't get messed up by vertical rotating
	// lookAt's forward vector must stay parallel with the ground
	Transform panner;       // used for panning
	Transform rotater;      // used for rotating
	Transform camera;		// used for zooming

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
		// Get children
		panner = transform.GetChild(0);
		rotater = panner.GetChild(0);
		camera = rotater.GetChild(0);

		// Set cursor lock state
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Panning
		if (Input.GetMouseButton(0))	// left click
		{
			panner.Translate(-Input.GetAxis("Mouse X") * panSensitivity, 0, -Input.GetAxis("Mouse Y") * panSensitivity, Space.Self);
		}

		// Rotating
		if (Input.GetMouseButton(1))    // right click
		{
			panner.Rotate(0, Input.GetAxis("Mouse X") * rotateSensitivity, 0, Space.Self);
			rotater.Rotate(-Input.GetAxis("Mouse Y") * rotateSensitivity, 0, 0, Space.Self);
		}

		// Zooming
		camera.Translate(0, 0, Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity, Space.Self);
	}
}
