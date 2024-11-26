using System;
using System.Collections.Generic;
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
			// when the player eats their first block, the block they ate and its surrounding eight blocks are guaranteed to not be mines
			// this is why the number of mines cannot exceed the number of blocks - 9
			throw new ArgumentException("Player's first action is impossible with the current amount of mines.");
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
		// Turn a random grass block into a mine
		int[] xy = GetRandomGrassBlockPosition();
		blocks[xy[1], xy[0]].SetType(Block.Type.MINE);
	}
	/// <summary> x and y refer to the center position of the 3x3 area. </summary>
	void PlaceMineNotIn3x3(int x, int y)
	{
		// Turn a random grass block that's not in the 3x3 into a mine
		while (true)
		{
			// Get a random grass block's position
			int[] xy = GetRandomGrassBlockPosition();

			// Ensure the position isn't in the 3x3
			if ((xy[0] >= x-1 && xy[0] <= x+1) && (xy[1] >= y-1 && xy[1] <= y+1))
			{
				continue;
			}

			// Turn the block into a mine
			blocks[xy[1], xy[0]].SetType(Block.Type.MINE);
			return;
		}
	}
	int[] GetRandomGrassBlockPosition()
	{
		// modified code from https://www.geeksforgeeks.org/cpp-implementation-minesweeper-game/ to create this function

		// Get a random grass block and return its position
		while (true)
		{
			// Get the position of a random block in terms of a 1D array
			int blockNum = UnityEngine.Random.Range(0, width * height);

			// Get the position of the block in terms of a 2D array
			int x = blockNum % width;
			int y = blockNum / width;  // integer division

			// Return the position if the block is grass
			if (blocks[y, x].GetBlockType() == Block.Type.GRASS)
			{
				return new int[] { x, y };
			}
		}
	}

	public void OnEat(int x, int y)
	{
		// Handle special case for when it's the player's first action
		if (playerOnFirstAction)
		{
			ReplaceMinesIn3x3(x, y);
			playerOnFirstAction = false;
		}

		blocks[y, x].SetNearbyMinesText(getMinePositionsIn3x3(x, y).Count);
	}
	/// <summary> x and y refer to the center position of the 3x3 area. </summary>
	public void ReplaceMinesIn3x3(int x, int y)
	{
		// Get the mines in the 3x3 and replace them
		foreach (int[] xy in getMinePositionsIn3x3(x, y))
		{
			blocks[xy[1], xy[0]].SetType(Block.Type.GRASS);
			PlaceMineNotIn3x3(x, y);
		}
	}
	/// <summary> x and y refer to the center position of the 3x3 area. </summary>
	List<int[]> getMinePositionsIn3x3(int x, int y)
	{
		// Create a list to store the positions of the mines we find
		List<int[]> positions = new List<int[]>();

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
					positions.Add(new int[] { x + xOffset, y + yOffset });
				}
			}
		}

		// Return positions
		return positions;
	}
}