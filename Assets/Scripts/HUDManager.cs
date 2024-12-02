using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
	[Header("HUD Elements")]
	[SerializeField] TMP_Text grassLeftText;
	[SerializeField] TMP_Text winLossText;

	[Header("Singleton Pattern")]
	private static HUDManager instance;
	public static HUDManager Instance { get { return instance; } }
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
		GameManager.OnWinGame += OnWinGame;
		GameManager.OnLoseGame += OnLoseGame;
	}
	void OnDisable()
	{
		GameManager.OnWinGame -= OnWinGame;
		GameManager.OnLoseGame -= OnLoseGame;
	}

	public void SetGrassLeftText(int grassLeft)
	{
		grassLeftText.text = "Grass left: " + grassLeft;
	}

	public void OnWinGame()
	{
		// Show win text
		winLossText.gameObject.SetActive(true);
		winLossText.text = "You Win!";
	}
	public void OnLoseGame()
	{
		// Show lose text
		winLossText.gameObject.SetActive(true);
		winLossText.text = "You Lose...";
	}
}
