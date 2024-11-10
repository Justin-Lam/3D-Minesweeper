using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
	[SerializeField] float panSensitivity;
	[SerializeField] float zoomSensitivity;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Panning
		if (Input.GetMouseButton(0))	// left click
		{
			transform.Translate(Input.GetAxis("Mouse X") * panSensitivity, Input.GetAxis("Mouse Y") * panSensitivity, 0, Space.Self);
		}

		// Zooming
		transform.Translate(0, 0, Input.GetAxis("Mouse Scroll Wheel") * zoomSensitivity, Space.Self);
	}

}
