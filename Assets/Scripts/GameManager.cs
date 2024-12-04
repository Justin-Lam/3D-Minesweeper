using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Level")]
	[SerializeField] protected int width;
	[SerializeField] protected int height;
	[SerializeField] protected int numMines;
	protected Block[,] blocks;

	[Header("Aesthetics")]
	[SerializeField] float perlinFrequency;
	[SerializeField] float perlinAmplitude;
	[SerializeField] float generationDelay;
	[SerializeField] float spawnHeight;
	[SerializeField] float spawnDuration;
	float perlinOffset;

	[Header("Prefabs")]
	[SerializeField] protected GameObject block;
	[SerializeField] protected GameObject stone;
	[SerializeField] protected GameObject fence;

	[Header("HUD")]
	protected int grassLeft;

	[Header("Gameplay")]
	protected bool playerOnFirstAction = true;
	// learned how to do this from https://www.reddit.com/r/gamedev/comments/u3hz2v/how_to_use_events_a_supersimple_unity_example/?rdt=39506
	// and by asking ChatGPT: "how do i change my GameManager to communicate to my HUDManager via events instead of public functions"
	public static event Action OnWinGame;
	public static event Action OnLoseGame;

	[Header("Singleton Pattern")]
	private static GameManager instance;
	public static GameManager Instance { get { return instance; } }
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
	void OnEnable()
	{
		Block.OnBlockEaten += OnBlockEaten;
		Block.OnExplode += LoseGame;
	}
	void OnDisable()
	{
		Block.OnBlockEaten -= OnBlockEaten;
		Block.OnExplode -= LoseGame;
	}

	protected virtual void Start()
	{
		ValidateParameters();
		InitializeGameplayVariables();
		CreateBlocks();
		StartCoroutine(CreateBarrier());
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

	protected virtual void InitializeGameplayVariables()
	{
		perlinOffset = UnityEngine.Random.Range(0, 1000000);	// arbitrary one million
		grassLeft = width * height - numMines;
		HUDManager.Instance.SetGrassLeftText(grassLeft);
	}

	void CreateBlocks()
	{
		// Instantiate blocks
		blocks = new Block[height, width];

		// Calculate offsets so the grid can be centered at (0, 0, 0)
		float offsetX = (width % 2 == 0) ? 0.5f : 0f;
		float offsetY = (height % 2 == 0) ? 0.5f : 0f;

		// Create blocks
		for (int y = 0; y < height; y++)    // note: y here corresponds to the z axis of the game world
		{
			for (int x = 0; x < width; x++)
			{
				float blockX = x - width / 2 + offsetX;
				float blockZ = y - height / 2 + offsetY;
				float blockY = LevelGenPerlin(blockX, blockZ);
				GameObject blockGO = Instantiate(block, new Vector3(blockX, blockY, blockZ), Quaternion.identity, transform);
				Block blockScript = blockGO.GetComponent<Block>();
				blockScript.SetPosition(x, y);
				blocks[y, x] = blockScript;
			}
		}
	}
	/// <summary>
	/// Places stones and fences around the grid of blocks
	/// Places them one by one, going stone-fence-stone-fence etc., going clockwise.
	/// </summary>
	IEnumerator CreateBarrier()
    {
		Vector3 block_TL = blocks[blocks.GetLength(0) - 1, 0].gameObject.transform.position;
		Vector3 block_TR = blocks[blocks.GetLength(0) - 1, blocks.GetLength(1) - 1].gameObject.transform.position;
		Vector3 block_BR = blocks[0, blocks.GetLength(1) - 1].gameObject.transform.position;
		Vector3 block_BL = blocks[0, 0].gameObject.transform.position;

		GameObject lastObjPlaced = null;

		for (int x = 0; x < width + 1; x++)
		{
			float stoneX = block_TL.x + x;
			float stoneZ = block_TL.z + 1;
			float stoneY = LevelGenPerlin(stoneX, stoneZ);
			Vector3 stonePosition = new Vector3(stoneX, stoneY, stoneZ);
			Vector3 fencePosition = stonePosition + new Vector3(0.5f, 1, 0);

			lastObjPlaced = Instantiate(stone, stonePosition, Quaternion.identity, transform);
			StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			yield return new WaitForSeconds(generationDelay);

			lastObjPlaced = Instantiate(fence, fencePosition, Quaternion.identity, transform);
			if (x < width)
			{
				StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			}
			yield return new WaitForSeconds(generationDelay);
		}
		lastObjPlaced.transform.Rotate(0, 90, 0);
		lastObjPlaced.transform.position += new Vector3(-0.5f, 0, -0.5f);
		StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
		yield return new WaitForSeconds(generationDelay);

		for (int y = 0; y < height + 1; y++)
		{
			float stoneX = block_TR.x + 1;
			float stoneZ = block_TR.z - y;
			float stoneY = LevelGenPerlin(stoneX, stoneZ);
			Vector3 stonePosition = new Vector3(stoneX, stoneY, stoneZ);
			Vector3 fencePosition = stonePosition + new Vector3(0, 1, -0.5f);

			lastObjPlaced = Instantiate(stone, stonePosition, Quaternion.identity, transform);
			StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			yield return new WaitForSeconds(generationDelay);

			lastObjPlaced = Instantiate(fence, fencePosition, Quaternion.identity, transform);
			lastObjPlaced.transform.Rotate(0, 90, 0);
			if (y < height)
			{
				StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			}
			yield return new WaitForSeconds(generationDelay);
		}
		lastObjPlaced.transform.Rotate(0, 90, 0);
		lastObjPlaced.transform.position += new Vector3(-0.5f, 0, 0.5f);
		StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
		yield return new WaitForSeconds(generationDelay);

		for (int x = 0; x < width + 1; x++)
		{
			float stoneX = block_BR.x - x;
			float stoneZ = block_BR.z - 1;
			float stoneY = LevelGenPerlin(stoneX, stoneZ);
			Vector3 stonePosition = new Vector3(stoneX, stoneY, stoneZ);
			Vector3 fencePosition = stonePosition + new Vector3(-0.5f, 1, 0);

			lastObjPlaced = Instantiate(stone, stonePosition, Quaternion.identity, transform);
			StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			yield return new WaitForSeconds(generationDelay);

			lastObjPlaced = Instantiate(fence, fencePosition, Quaternion.identity, transform);
			if (x < width)
			{
				StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			}
			yield return new WaitForSeconds(generationDelay);
		}
		lastObjPlaced.transform.Rotate(0, 90, 0);
		lastObjPlaced.transform.position += new Vector3(0.5f, 0, 0.5f);
		StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
		yield return new WaitForSeconds(generationDelay);

		for (int y = 0; y < height + 1; y++)
		{
			float stoneX = block_BL.x - 1;
			float stoneZ = block_BL.z + y;
			float stoneY = LevelGenPerlin(stoneX, stoneZ);
			Vector3 stonePosition = new Vector3(stoneX, stoneY, stoneZ);
			Vector3 fencePosition = stonePosition + new Vector3(0, 1, 0.5f);

			lastObjPlaced = Instantiate(stone, stonePosition, Quaternion.identity, transform);
			StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			yield return new WaitForSeconds(generationDelay);

			lastObjPlaced = Instantiate(fence, fencePosition, Quaternion.identity, transform);
			lastObjPlaced.transform.Rotate(0, 90, 0);
			if (y < height)
			{
				StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
			}
			yield return new WaitForSeconds(generationDelay);
		}
		lastObjPlaced.transform.Rotate(0, 90, 0);
		lastObjPlaced.transform.position += new Vector3(0.5f, 0, -0.5f);
		StartCoroutine(lastObjPlaced.GetComponent<JuicySpawn>().FallIntoPlace(spawnHeight, spawnDuration));
		yield return new WaitForSeconds(generationDelay);
	}
	/// <summary> Returns the y position to set a game object to given its x and z. </summary>
	float LevelGenPerlin(float x, float z)
	{
		float result;
		result = Mathf.PerlinNoise((x + perlinOffset) * perlinFrequency, (z + perlinOffset) * perlinFrequency);
		result -= 0.5f; // adjust range of values from [0, 1] to [-0.5, 0.5]
		result *= perlinAmplitude;
		return result;
	}

	protected virtual void PlaceMines()
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
			if ((xy[0] >= x - 1 && xy[0] <= x + 1) && (xy[1] >= y - 1 && xy[1] <= y + 1))
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

	// Used to choose map values for tutorial
	void PrintMinePos()
    {
		for (int y = 0; y < height; y++)    // note: y here corresponds to the z axis of the game world
		{
			for (int x = 0; x < width; x++)
			{
				if (blocks[y, x].GetBlockType() == Block.Type.MINE)
                {
					Debug.Log("y = " + y + " and x = " + x);
                }
			}
		}
	}

	protected virtual void OnBlockEaten(int x, int y)
	{
		// Handle special case for when it's the player's first action
		if (playerOnFirstAction)
		{
			ReplaceMinesIn3x3(x, y);
			playerOnFirstAction = false;

			// PrintMinePos();
		}

		if (blocks[y, x].GetBlockType() == Block.Type.GRASS)
		{
			blocks[y, x].SetNearbyMinesText(getMinePositionsIn3x3(x, y).Count);

			grassLeft--;
			HUDManager.Instance.SetGrassLeftText(grassLeft);
			if (grassLeft <= 0)
			{
				WinGame();
			}
		}

		blocks[y, x].HandleOnEat();
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
	protected List<int[]> getMinePositionsIn3x3(int x, int y)
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

	protected void WinGame()
	{
		OnWinGame?.Invoke();
	}
	public void LoseGame()
	{
		OnLoseGame?.Invoke();
	}
}