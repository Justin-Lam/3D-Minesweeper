using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : GameManager
{
	public DialogueManager dialogueManager;
	public GameObject playerObject;

	[Header("Demonstration Block Coords")]
	[SerializeField] int demoBlockX = 2;
	[SerializeField] int demoBlockY = 1;

	[Header("Materials")]
	[SerializeField] Material highlight;
	[SerializeField] Material mine;

	int[][] minePos;
	int[] firstBlock;
	Player playerScript;
	Rigidbody playerRb;

	// dialogue event variables
	int grassEaten = 0;
	int minesEaten = 0;


	protected override void Start()
	{
		base.Start();

		playerScript = playerObject.GetComponent<Player>();
		playerRb = playerObject.GetComponent<Rigidbody>();

		playerScript.enabled = false;

		// start up the dialogue
		dialogueManager.CallNextLine();

		Debug.Log(blocks);
	}

	protected override void InitializeGameplayVariables()
	{
		base.InitializeGameplayVariables();

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
		// Turn grass block into a mine
		blocks[xy[1], xy[0]].SetType(Block.Type.MINE);
	}

	protected override void OnBlockEaten(int x, int y)
	{
		Debug.Log("x: " + x + ", y: " + y);
		if (blocks[y, x].GetBlockType() == Block.Type.GRASS)
		{
			if (grassEaten == 0) // for the eating tutorial
			{
				grassEaten = 1;
				dialogueManager.CallNextLine();
			}
			else if (grassEaten == 1) // for the numbering tutorial
			{
				if (x != demoBlockX && y != demoBlockY)
                {
					return; // if it's not the highlighted block then nothing happens on eat
                }
				grassEaten = 2;
				dialogueManager.CallNextLine();
			}

			blocks[y, x].SetNearbyMinesText(getMinePositionsIn3x3(x, y).Count);

			grassLeft--;
			HUDManager.Instance.SetGrassLeftText(grassLeft);
			if (grassLeft <= 0)
			{
				WinGame();
			}

			blocks[y, x].HandleOnEat();
		}
		else if (blocks[y, x].GetBlockType() == Block.Type.MINE)
        {
			if (grassEaten < 2) // mines are only interactable after the second block has been eaten (triggers free roam)
            {
				return;
            }

			if (minesEaten == 0) // for the flagging tutorial
			{
				minesEaten = 1;
				dialogueManager.CallNextLine();
			}
			else if (minesEaten == 1) // for the rest of the mines
			{
				minesEaten = 2;
				dialogueManager.CallNextLine();
			}

			MeshRenderer mr = blocks[y, x].GetComponent<MeshRenderer>();
			mr.material = mine;
		}
	}

	public bool CheckPrecondition(Precondition precondition)
    {
		switch (precondition)
        {
			case Precondition.firstBlockEaten:
				if (grassEaten > 0)
					return true;
				return false;
			case Precondition.secondBlockEaten:
				if (grassEaten > 1)
					return true;
				return false;
			case Precondition.firstMineEaten:
				if (minesEaten > 0)
					return true;
				return false;
			case Precondition.mineEaten:
				if (minesEaten > 1)
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
				MeshRenderer mr = blocks[demoBlockY, demoBlockX].GetComponent<MeshRenderer>();
				mr.material = highlight;
				break;
			default:
				break;
		}
	}
}