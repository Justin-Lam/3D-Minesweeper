using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Debuggin")]
	[SerializeField] TMP_InputField xInputField;
	[SerializeField] TMP_InputField yInputField;

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
		CreateBlocks();
		PlaceMines();
	}

	void CreateBlocks()
	{
		// Instantiate blocks
		blocks = new Block[height, width];

		// Calculate offsets so the grid can be centered at (0, 0, 0)
		float offsetX = (width % 2 == 0) ? 0.5f : 0f;
		float offsetY = (height % 2 == 0) ? 0.5f : 0f;

		// Create blocks
		for (int y = 0; y < height; y++)	// note: y corresponds to the z axis
		{
			for (int x = 0; x < width; x++)
			{
				Vector3 position = new Vector3(x - width/2 + offsetX, 0.5f, y - height/2 + offsetY);
				GameObject blockGO = Instantiate(block, position, Quaternion.identity, transform);
				Block blockScript = blockGO.GetComponent<Block>();
				blockScript.SetPosition(x, y);
				blocks[y, x] = blockScript;
			}
		}
	}

	void PlaceMines()
	{
		// modified code from https://www.geeksforgeeks.org/cpp-implementation-minesweeper-game/ to create this function

		// Loop until we've placed all the mines (notice there's no i++ in the loop header)
		for (int i = 0; i < numMines;)
		{
			// Get the position of the block to turn into a mine in terms of a 1D array
			int blockNum = Random.Range(0, width*height);

			// Get the position of the block in terms of a 2D array
			int x = blockNum % width;
			int y = blockNum / height;  // integer division

			// Turn the block into a mine if it isn't one already
			// only advance the loop counter (i) if we've successfully placed a new mine
			if (blocks[y, x].IsGrass())
			{
				blocks[y, x].BecomeMine();
				i++;
			}
		}
	}

	public void OnEat()
	{
		int x = int.Parse(xInputField.text);
		int y = int.Parse(yInputField.text);


	}
}
