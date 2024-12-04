// wrote this script with minor help from ChatGPT and https://gamedevbeginner.com/how-to-zoom-a-camera-in-unity-3-methods-with-examples/#movement_zoom

using System;
using UnityEngine;

public class GameplayCamera : MonoBehaviour
{

	[Header("Panning")]
	[SerializeField] float panSensitivity;
	[SerializeField] float panDrag;
	float relativePanSensitivity;           // relative to zoom (pan less when zoomed in, pan more when zoomed out)
	Vector3 panVelocity;

	[Header("Rotating")]
	[SerializeField] float rotateSensitivity;
	[SerializeField] float rotateDrag;
	Vector3 rotateVelocity;
	float previousRotationSegment = 0;
	public static event Action<int> OnCameraRotatedIntoNewSegment;

	[Header("Zooming")]
	[SerializeField] float initialZoom;     // initial dist cam is from panner
	[SerializeField] float zoomSensitivity;
	[SerializeField] float zoomSpeed;
	[SerializeField] float maxZoomIn;       // min dist cam can be from panner
	[SerializeField] float maxZoomOut;      // max dist cam can be from panner
	float targetZoom;                       // dist cam should be from panner
	float currentZoom;                      // dist cam is from panner

	[Header("Children")]
	// this double parent setup for the camera is necessary so vertical panning doesn't get messed up by vertical rotating
	// the panner's forward vector must always stay parallel with the ground
	Transform panner;       // for panning and rotating horizontally
	Transform rotater;      // for rotating vertically
	Transform cam;          // for zooming

	[Header("Singleton Pattern")]
	private static GameplayCamera instance;
	public static GameplayCamera Instance { get { return instance; } }
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
		if (Input.GetMouseButton(0))    // left click
		{
			relativePanSensitivity = panSensitivity * (currentZoom / initialZoom);
			float dx = -Input.GetAxis("Mouse X") * relativePanSensitivity;
			float dz = -Input.GetAxis("Mouse Y") * relativePanSensitivity;
			panVelocity = new Vector3(dx, 0, dz);
		}
		else
		{
			// got the idea of lerping a velocity vector to 0 from ChatGPT:
			// "what's the best way to add acceleration and deceleration to my camera panning? rigidbody? or code it myself?"
			panVelocity = Vector3.Lerp(panVelocity, Vector3.zero, panDrag * Time.deltaTime);
		}
		panner.Translate(panVelocity, Space.Self);
		//panner.position += panVelocity;

		// Rotating the camera
		if (Input.GetMouseButton(1))    // right click
		{
			float dx = -Input.GetAxis("Mouse Y") * rotateSensitivity;
			float dy = Input.GetAxis("Mouse X") * rotateSensitivity;
			rotateVelocity = new Vector3(dx, dy, 0);
		}
		else
		{
			rotateVelocity = Vector3.Lerp(rotateVelocity, Vector3.zero, rotateDrag * Time.deltaTime);
		}
		panner.Rotate(0, rotateVelocity.y, 0, Space.Self);
		rotater.Rotate(rotateVelocity.x, 0, 0, Space.Self);

		// Rotating the blocks
		int currentRotationSegment = GetRotationSegment();
		if (previousRotationSegment != currentRotationSegment)
		{
			OnCameraRotatedIntoNewSegment?.Invoke(currentRotationSegment);
		}

		// Zooming
		targetZoom -= Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity;
		targetZoom = Mathf.Clamp(targetZoom, maxZoomIn, maxZoomOut);
		currentZoom = Mathf.MoveTowards(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
		cam.position = panner.position + cam.forward * -currentZoom;

		// Set previousRotationSegment (this must come at the end of Update() so the next Update() call can use it)
		previousRotationSegment = currentRotationSegment;
	}

	int GetRotationSegment()
	{
		// got this formula from ChatGPT
		// returns a number between 0 and 3
		// 0: 315 < y or y <= 45
		// 1: 45 < y <= 135
		// 2: 135 < y <= 225
		// 3: 225 < y <= 315
		// where y is the y rotation of the panner in degrees
		return Mathf.RoundToInt(panner.rotation.eulerAngles.y / 90) % 4;
	}
}