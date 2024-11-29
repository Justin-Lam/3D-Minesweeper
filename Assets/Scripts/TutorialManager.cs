using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : GameManager
{
	int[][] minePos;
	int[] firstBlock;

	protected override void InitializeGameplayVariables()
	{
		grassLeft = width * height - numMines;
		HUDManager.Instance.SetGrassLeftText(grassLeft);

		// Manually set where the mines will be (tutorial level is always the same)
		minePos = new int[numMines][];
		minePos[0] = new int[] { 0, 1 };
		minePos[1] = new int[] { 0, 2 };
		minePos[2] = new int[] { 0, 3 };
		minePos[3] = new int[] { 0, 4 };
		minePos[4] = new int[] { 0, 5 };
		minePos[5] = new int[] { 1, 1 };
		minePos[6] = new int[] { 2, 1 };
		minePos[7] = new int[] { 3, 1 };
		minePos[8] = new int[] { 4, 1 };
		minePos[9] = new int[] { 5, 1 };
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

	public override void OnBlockEaten(int x, int y)
	{
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
}