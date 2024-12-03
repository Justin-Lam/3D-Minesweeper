using TMPro;
using System;
using UnityEngine;

public class Block : MonoBehaviour
{
	public enum Type { GRASS, MINE };

	[Header("Data")]
	Type type = Type.GRASS; public Type GetBlockType() { return type; }
	int x;
	int y;
	bool eaten = false;

	[Header("Explosion")]
	[SerializeField] float radius = 5f;
	[SerializeField] float upwardsModifier = 3f;
	[SerializeField] float power = 500f;
	public static event Action<int, int> OnBlockEaten;
	public static event Action OnExplode;

	[Header("Text")]
	[SerializeField] Color[] numberColors = new Color[8];
	TMP_Text nearbyMinesText;

	[Header("Materials")]
	[SerializeField] Material dirt;
	[SerializeField] Material mine;
	MeshRenderer mr;

	void OnEnable()
	{
		GameplayCamera.OnCameraRotatedIntoNewSegment += RotateIntoSegment;
	}
	void OnDisable()
	{
		GameplayCamera.OnCameraRotatedIntoNewSegment -= RotateIntoSegment;
	}

	void Start()
	{
		// Get nearby mines text
		nearbyMinesText = GetComponentInChildren<TMP_Text>();

		// Get renderer
		mr = GetComponent<MeshRenderer>();
	}

	public void SetPosition(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	public void SetType(Type type)
	{
		this.type = type;
	}
	public void SetNearbyMinesText(int numMines)
	{
		if (numMines != 0)
		{
			nearbyMinesText.text = numMines.ToString();
			nearbyMinesText.color = numberColors[numMines - 1];
		}
	}

	public void OnEat()
	{
		// Do some stuff then tell the game manager that this block was eaten
		// The game manager is going to do some stuff and then come back to this block and tell it to finish what it needs to do ( HandleOnEat() )

		if (!eaten)
		{
			eaten = true;
			OnBlockEaten?.Invoke(x, y);
		}
	}
	public void HandleOnEat()
	{
		if (type == Type.GRASS)
		{
			mr.material = dirt;
		}
		else
		{
			OnExplode?.Invoke();

			mr.material = mine;

			Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
			foreach (Collider hit in colliders)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();

				if (rb)
				{
					rb.AddExplosionForce(power, transform.position, radius, upwardsModifier, ForceMode.Impulse);
				}
			}
		}
	}

	void RotateIntoSegment(int cameraRotationSegment)
	{
		// Rotate the block to face the segment the camera is in so the player can read the block's nearby mines
		transform.rotation = Quaternion.Euler(0, cameraRotationSegment * 90, 0);
	}
}