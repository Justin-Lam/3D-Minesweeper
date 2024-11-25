using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Level Generation")]
	[SerializeField] GameObject block;
	[SerializeField] int width;
	[SerializeField] int height;
	[SerializeField] int numMines;
	Block[,] blocks;

	[Header("Gameplay")]
	bool playerOnFirstAction = true;

	[Header("Singleton Pattern")]
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
		ValidateParameters();
		CreateBlocks();
		PlaceMines();
	}

	void ValidateParameters()
	{
		if (width < 0)
		{
			throw new ArgumentException("Width cannot be negative.");
		}
		if (height < 0)
		{
			throw new ArgumentException("Height cannot be negative.");
		}
		if (numMines > width * height - 9)
		{
			throw new ArgumentException("Player's first action is impossible with the current amount of mines.");
			// when the player eats their first block, the block they ate and its surrounding eight blocks are guaranteed to not be mines
			// this is why the number of mines cannot exceed the number of blocks - 9
		}
	}

	void CreateBlocks()
	{
		// Instantiate blocks
		blocks = new Block[height, width];

		// Calculate offsets so the grid can be centered at (0, 0, 0)
		float offsetX = (width % 2 == 0) ? 0.5f : 0f;
		float offsetY = (height % 2 == 0) ? 0.5f : 0f;

		// Create blocks
		for (int y = 0; y < height; y++)	// note: y here corresponds to the z axis of the game world
		{
			for (int x = 0; x < width; x++)
			{
				Vector3 position = new Vector3(x - width/2 + offsetX, 0, y - height/2 + offsetY);
				GameObject blockGO = Instantiate(block, position, Quaternion.identity, transform);
				Block blockScript = blockGO.GetComponent<Block>();
				blockScript.SetPosition(x, y);
				blocks[y, x] = blockScript;
			}
		}
	}

	void PlaceMines()
	{
		for (int i = 0; i < numMines; i++)
		{
			PlaceMine();
		}
	}
	void PlaceMine()
	{
		// modified code from https://www.geeksforgeeks.org/cpp-implementation-minesweeper-game/ to create this function

		// Randomly find a block that's grass and turn it into a mine
		while (true)
		{
			// Get the position of the block to turn into a mine in terms of a 1D array
			int blockNum = UnityEngine.Random.Range(0, width * height);

			// Get the position of the block in terms of a 2D array
			int x = blockNum % width;
			int y = blockNum / width;  // integer division

			// Turn the block into a mine if it isn't one already
			// only advance the loop counter (i) if we've successfully placed a new mine
			if (blocks[y, x].GetBlockType() == Block.Type.GRASS)
			{
				blocks[y, x].SetType(Block.Type.MINE);
				return;
			}
		}
	}
	void PlaceMineNotIn3x3(int x, int y)
	{
		// Randomly find a block that's grass and turn it into a mine
		while (true)
		{
			// Get the position of the block to turn into a mine in terms of a 1D array
			int blockNum = UnityEngine.Random.Range(0, width * height);

			// Get the position of the block in terms of a 2D array
			int blockX = blockNum % width;
			int blockY = blockNum / width;  // integer division

			// Ensure the position isn't in the 3x3
			if ((blockX >= x-1 && blockX <= x+1) && (blockY >= y-1 && blockY <= y+1))
			{
				continue;
			}

			// Turn the block into a mine if it isn't one already
			// only advance the loop counter (i) if we've successfully placed a new mine
			if (blocks[blockY, blockX].GetBlockType() == Block.Type.GRASS)
			{
				blocks[blockY, blockX].SetType(Block.Type.MINE);
				return;
			}
		}
	}

	public void OnEat(int x, int y)
	{
		if (playerOnFirstAction)
		{
			ReplaceMinesIn3x3(x, y);
		}

		blocks[y, x].SetNearbyMinesText(GetNumMinesIn3x3(x, y));

		playerOnFirstAction = false;
	}
	public void ReplaceMinesIn3x3(int x, int y)
	{
		// Scan the nine blocks for mines
		for (int yOffset = -1; yOffset <= 1; yOffset++)
		{
			// Don't go out of array bounds
			if (y + yOffset < 0 || y + yOffset > height - 1)
			{
				continue;
			}

			for (int xOffset = -1; xOffset <= 1; xOffset++)
			{
				// Don't go out of array bounds
				if (x + xOffset < 0 || x + xOffset > width - 1)
				{
					continue;
				}

				// Mine found
				if (blocks[y + yOffset, x + xOffset].GetBlockType() == Block.Type.MINE)
				{
					blocks[y + yOffset, x + xOffset].SetType(Block.Type.GRASS);
					PlaceMineNotIn3x3(x, y);
				}
			}
		}
	}
	int GetNumMinesIn3x3(int x, int y)
	{
		int result = 0;

		// Scan the nine blocks for mines
		for (int yOffset = -1; yOffset <= 1; yOffset++)
		{
			// Don't go out of array bounds
			if (y + yOffset < 0 || y + yOffset > height - 1)
			{
				continue;
			}

			for (int xOffset = -1; xOffset <= 1; xOffset++)
			{
				// Don't go out of array bounds
				if (x + xOffset < 0 || x + xOffset > width - 1)
				{
					continue;
				}

				// Mine found
				if (blocks[y + yOffset, x + xOffset].GetBlockType() == Block.Type.MINE)
				{
					result++;
				}
			}
		}
		return result;
	}
}
