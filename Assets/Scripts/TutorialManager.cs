using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	[Header("Explosion")]
	[SerializeField] float radius = 5f;
	[SerializeField] float upwardsModifier = 3f;
	[SerializeField] float power = 500f;

	[Header("Crow")]
	[SerializeField] GameObject crow;

	int[][] minePos;
	int[] firstBlock;
	Player playerScript;
	Rigidbody playerRb;
	private AudioSource crowSound;

	// dialogue event variables
	int grassEaten = 0;
	int minesEaten = 0;
	bool canPlaceFlags = false;
	bool flagPlaced = false;
	bool gameWon = false;
	Block lastBlock;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(SpawnCrow());

		playerScript = playerObject.GetComponent<Player>();
		playerRb = playerObject.GetComponent<Rigidbody>();

		playerScript.enabled = false;

		// start up the dialogue
		dialogueManager.CallNextLine();
	}
	IEnumerator SpawnCrow()
	{
		yield return new WaitForSeconds(2.5f);
		Instantiate(crow, new Vector3(-3, 20, 3), Quaternion.identity).transform.Rotate(0, -45, 0);
		yield return new WaitForSeconds(1.75f);
		crowSound = soundManager.GetComponents<AudioSource>()[8];
		crowSound.Play();
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

	protected override void OnFlagPlaced()
    {
		if (canPlaceFlags)
        {
			poopSound.Play();
			player.HandleOnFlag();

			if (!flagPlaced)
            {
				flagPlaced = true;
				dialogueManager.SetCurrentLine(41); // line right before the flagging line
				dialogueManager.CallNextLine();
			}
		}
	}

	protected override void OnBlockEaten(int x, int y)
	{
		// Debug.Log("x: " + x + ", y: " + y);
		if (blocks[y, x].GetBlockType() == Block.Type.GRASS)
		{
			if (grassEaten == 0) // for the eating tutorial
			{
				grassEaten = 1;
				dialogueManager.CallNextLine();
			}
			else if (grassEaten == 1) // for the numbering tutorial
			{
				if (x != demoBlockX || y != demoBlockY)
                {
					blocks[y, x].eaten = false;
					return; // if it's not the highlighted block then nothing happens on eat and set back to uneaten
                }
				grassEaten = 2;
				dialogueManager.CallNextLine();
			}

			blocks[y, x].SetNearbyMinesText(getMinePositionsIn3x3(x, y).Count);

			grassLeft--;
			eatSound.Play();

			HUDManager.Instance.SetGrassLeftText(grassLeft);
			if (grassLeft <= 0) // WIN CONDITION
			{
				gameWon = true;
				winSound.Play();
				dialogueManager.SetCurrentLine(47); // line right before the victory line
				dialogueManager.CallNextLine();
				lastBlock = blocks[y, x];
			}

			blocks[y, x].HandleOnEat();
		}
		else if (blocks[y, x].GetBlockType() == Block.Type.MINE)
        {
			if (grassEaten < 2) // mines are only interactable after the second block has been eaten (triggers free roam)
            {
				blocks[y, x].eaten = false;
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
				dialogueManager.SetCurrentLine(45); // line before the one that talks about the mine
				dialogueManager.CallNextLine();
			}

			eatSound.Play();
			MeshRenderer mr = blocks[y, x].GetComponent<MeshRenderer>();
			mr.materials = new Material[] { mine };
		}
	}

	void ExplodeEnd()
    {
		if (lastBlock != null)
        {
			explodeSound.Play();
			Collider[] colliders = Physics.OverlapSphere(lastBlock.transform.position, radius);
			foreach (Collider hit in colliders)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();

				if (rb)
				{
					if (rb.gameObject.CompareTag("Player"))
					{
						Player.Instance.OnAffectedByExplosion();
					}

					rb.gameObject.GetComponent<AffectableByExplosion>()?.OnAffectedByExplosion();

					rb.AddExplosionForce(power, lastBlock.transform.position, radius, upwardsModifier, ForceMode.Impulse);
				}
			}

			Cursor.lockState = CursorLockMode.None;
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
                {
					minesEaten--;
					return true;
				}
				return false;
			case Precondition.firstFlag:
				return flagPlaced;
			case Precondition.win:
				return gameWon;
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
				mr.materials = new Material[] { highlight };
				break;
			case DialogueEvent.allowFlag:
				canPlaceFlags = true;
				break;
			case DialogueEvent.explode:
				ExplodeEnd();
				break;
			case DialogueEvent.endTutorial:
				SceneManager.LoadScene("TitleScreen");
				break;
			default:
				break;
		}
	}
}