using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	[Header("Panning")]
	[SerializeField] float panSensitivity;
	float relativePanSensitivity;			// relative to zoom (pan less when zoomed in, pan more when zoomed out)

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
	// this double parent setup for the camera is necessary so vertical panning doesn't get messed up by vertical rotating
	// the panner's forward vector must always stay parallel with the ground
	Transform panner;       // for panning
	Transform rotater;      // for rotating
	Transform cam;          // for zooming


	void Start()
	{
		// Set panning variables
		relativePanSensitivity = panSensitivity;

		// Set zooming variables
		currentZoom = initialZoom;
		targetZoom = initialZoom;

		// Get children
		panner = transform.GetChild(0);
		rotater = panner.GetChild(0);
		cam = rotater.GetChild(0);

		// Set cursor lock state
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Panning
		if (Input.GetMouseButton(0))	// left click
		{
			relativePanSensitivity = panSensitivity * (currentZoom / initialZoom);
			panner.Translate(-Input.GetAxis("Mouse X") * relativePanSensitivity, 0, -Input.GetAxis("Mouse Y") * relativePanSensitivity, Space.Self);
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
