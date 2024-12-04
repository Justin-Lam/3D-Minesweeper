using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : GameManager
{
	public DialogueManager dialogueManager;
	public GameObject playerObject;

	[Header("Demonstration Block Coords")]
	[SerializeField] int demoBlockX;
	[SerializeField] int demoBlockY;

	int[][] minePos;
	int[] firstBlock;
	Player playerScript;
	Rigidbody playerRb;

	// dialogue event variables
	int blockEaten = 0;


	protected override void Start()
	{
		base.Start();

		playerScript = playerObject.GetComponent<Player>();
		playerRb = playerObject.GetComponent<Rigidbody>();

		playerScript.enabled = false;

		// start up the dialogue
		dialogueManager.CallNextLine();
	}

	protected override void InitializeGameplayVariables()
	{
		grassLeft = width * height - numMines;
		HUDManager.Instance.SetGrassLeftText(grassLeft);

		// Manually set where the mines will be (tutorial level is always the same)
		minePos = new int[numMines][];
		minePos[0] = new int[] { 0, 1 };
		minePos[1] = new int[] { 0, 3 };
		minePos[2] = new int[] { 2, 0 };
	}

	protected override void PlaceMines()
	{
		for (int i = 0; i < numMines; i++)
		{
            PlaceMine(minePos[i]);
		}
	}
	void PlaceMine(int[] xy)
	{
		// Turn a random grass block into a mine
		blocks[xy[1], xy[0]].SetType(Block.Type.MINE);
	}

	protected override void OnBlockEaten(int x, int y)
	{
		if (blockEaten == 0)
        {
			blockEaten = 1;
			dialogueManager.CallNextLine();
		}
		else if (blockEaten == 1)
        {
			blockEaten = 2;
			dialogueManager.CallNextLine();
		} // also make it so that only target block can be eaten

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

	public bool CheckPrecondition(Precondition precondition)
    {
		switch (precondition)
        {
			case Precondition.firstBlockEaten:
				if (blockEaten > 0)
					return true;
				return false;
			case Precondition.secondBlockEaten:
				if (blockEaten > 1)
					return true;
				return false;
			default:
				return false;
        }
    }

	public void ToggleEvent(DialogueEvent dialogueEvent)
    {
		switch (dialogueEvent)
        {
			case DialogueEvent.allowEat:
				playerScript.enabled = true;
				playerRb.constraints = RigidbodyConstraints.FreezeAll;
				break;
			case DialogueEvent.allowMove:
				playerRb.constraints = RigidbodyConstraints.None;
				playerRb.constraints = RigidbodyConstraints.FreezeRotation;
				// also make it so that target block is highlighted
				break;
			default:
				break;
		}
	}
}