using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Level Generation")]
	[SerializeField] GameObject block;
	[SerializeField] int width;
	[SerializeField] int height;
	[SerializeField] int numMines;
	Block[,] blocks;

	// Singleton Pattern
	private static GameManager instance;
	public static GameManager Instance {  get { return instance; } }
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
		GenerateLevel();
	}

	void GenerateLevel()
	{
		// Instantiate blocks
		blocks = new Block[height, width];

		// Calculate offsets so the grid can be centered at (0, 0, 0)
		float offsetX = (width % 2 == 0) ? 0.5f : 0f;
		float offsetZ = (height % 2 == 0) ? 0.5f : 0f;

		// Create blocks
		for (int z = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				Vector3 position = new Vector3(x - width/2 + offsetX, 0.5f, z - height/2 + offsetZ);
				GameObject go = Instantiate(block, position, Quaternion.identity, transform);
				blocks[z, x] = go.GetComponent<Block>();
			}
		}
	}
}
