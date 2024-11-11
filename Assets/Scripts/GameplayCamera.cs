using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	[Header("Panning")]
	[SerializeField] float panSensitivity;

	[Header("Zooming")]
	[SerializeField] float initialZoom;		// initial dist cam is from panner
	[SerializeField] float zoomSensitivity;
	[SerializeField] float zoomSpeed;
	[SerializeField] float maxZoomIn;		// min dist cam can be from panner
	[SerializeField] float maxZoomOut;		// max dist cam can be from panner
	float targetZoom;						// dist cam should be from panner
	float currentZoom;						// dist cam is from panner

	[Header("Rotating")]
	[SerializeField] float rotateSensitivity;

	[Header("Children")]
	// this double parent setup is necessary so vertical panning doesn't get messed up by vertical rotating
	// the panner's forward vector must stay parallel with the ground
	Transform panner;       // for panning
	Transform rotater;      // for rotating
	Transform cam;          // for zooming


	void Start()
	{
		// Get children
		panner = transform.GetChild(0);
		rotater = panner.GetChild(0);
		cam = rotater.GetChild(0);

		// Set current and target zoom levels
		currentZoom = initialZoom;
		targetZoom = initialZoom;

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
		targetZoom -= Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity;
		targetZoom = Mathf.Clamp(targetZoom, maxZoomIn, maxZoomOut);
		currentZoom = Mathf.MoveTowards(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
		cam.position = panner.position + cam.forward * -currentZoom;
	}
}
